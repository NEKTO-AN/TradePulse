using System.Reflection;
using Application.Helpers.Configuration;
using Application.Services.DataConsumerService;
using Domain.Orderbook;
using Domain.PumpDumpSnapshot;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Application.Test.Services.DataConsumerService
{
    public class DataConsumerServiceTest
    {
        private readonly string _symbol = "BTCUSDT";
        private readonly Mock<IOrderbookRepository> _orderbookRepositoryMock = new();
        private readonly Mock<IPumpDumpSnapshotRepository> _pumpDumpSnapshotRepositoryMock = new();
        private Mock<AppConfiguration> _appConfigurationMock;

        private DataConsumerWorkerService _dataConsumerWorkerService;
        
        [SetUp]
        public void Setup()
        {
            Mock<IConfigurationSection> _symbolsConfigurationSection = new();
            _symbolsConfigurationSection.Setup(x => x.Value).Returns(_symbol);
            Mock<IConfigurationSection> _topicsConfigurationSection = new();
            _topicsConfigurationSection.Setup(x => x.Value).Returns("orderbook.500");

            Mock<IConfiguration> _configuration = new();
            _configuration.Setup(x => x["KAFKA_ORDERBOOK_TOPIC"]).Returns("some-topic");
            _configuration.Setup(x => x["KAFKA_ENDPOINT"]).Returns("some-endpoint");
            _configuration.Setup(x => x.GetSection("Symbols").GetChildren()).Returns(new IConfigurationSection[] { _symbolsConfigurationSection.Object });
            _configuration.Setup(x => x.GetSection("Topics")).Returns(_topicsConfigurationSection.Object);

            _appConfigurationMock = new(_configuration.Object);

            _dataConsumerWorkerService = new(_orderbookRepositoryMock.Object, _pumpDumpSnapshotRepositoryMock.Object, _appConfigurationMock.Object);
        }

        [Test]
        public async Task AddOrderbookItemAsync_Positive_Test()
        {
            long lastUpdateTs = 12345768;
            OrderbookDataMessage orderbookDataMessageEntity = new(
                timestamp: lastUpdateTs,
                data: new(
                    symbol: "BTCUSDT",
                    bids: new double[][]
                    {
                        new double[] {10, 12},
                        new double[] {9, 12},
                    },
                    asks: new double[][]
                    {
                        new double[] {11, 12},
                        new double[] {12, 12},
                    },
                    crossSequence: 1,
                    updateId: 1
                )
            )
            {
                LastPrice = 123
            };
            Domain.Orderbook.Orderbook orderbook = new(orderbookDataMessageEntity.Timestamp, orderbookDataMessageEntity.Data!);
            _orderbookRepositoryMock.Setup(x => x.AddAsync(orderbook, default))
                .Returns(Task.CompletedTask);

            await _dataConsumerWorkerService.AddOrderbookItemAsync(new AddOrderbookItemQuery(orderbookDataMessageEntity));

            // Access private field using reflection
            FieldInfo? symbolBSTInfo = typeof(DataConsumerWorkerService).GetField("symbolBST", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<string, PriceBinarySearchTree>? value = symbolBSTInfo!.GetValue(_dataConsumerWorkerService) as Dictionary<string, PriceBinarySearchTree>;


            Assert.That(value, Is.Not.EqualTo(null));
            Assert.That(value[_symbol].SearchPrice(123)!.LastUpdateTs, Is.EqualTo(lastUpdateTs));

            Assert.Pass();
        }

        [Test]
        public async Task AddOrderbookItemAsync_WithNullDataModel_Negative_Test()
        {
            long lastUpdateTs = 12345768;
            OrderbookDataMessage orderbookDataMessageEntity = new(timestamp: lastUpdateTs, data: null)
            {
                LastPrice = 123
            };

            try
            {
                await _dataConsumerWorkerService.AddOrderbookItemAsync(new AddOrderbookItemQuery(orderbookDataMessageEntity));
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Data model is empty"));
            }
        }

        [Test]
        public async Task FindAnomalyAsync_Positive_Test()
        {
            double[] prices = new double[]{ 1289, 1314, 1330 };
            for (int i = 0; i < prices.Length; i++)
            {
                OrderbookDataMessage orderbookDataMessageEntity = new(
                    timestamp: (long)TimeSpan.FromMinutes(i + 1).TotalMilliseconds,
                    data: new(
                        symbol: _symbol,
                        bids: Array.Empty<double[]>(),
                        asks: Array.Empty<double[]>(),
                        crossSequence: 1,
                        updateId: 1
                    )
                )
                {
                    LastPrice = prices[i]
                };


                Domain.Orderbook.Orderbook orderbook = new(orderbookDataMessageEntity.Timestamp, orderbookDataMessageEntity.Data!);
                _orderbookRepositoryMock.Setup(x => x.AddAsync(orderbook, default))
                    .Returns(Task.CompletedTask);

                await _dataConsumerWorkerService.AddOrderbookItemAsync(new AddOrderbookItemQuery(orderbookDataMessageEntity));
            }

            Domain.PumpDumpSnapshot.PumpDumpSnapshot? result = await _dataConsumerWorkerService.FindAnomalyAsync(new FindAnomalyQuery(_symbol));

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.Multiple(() =>
            {
                Assert.That(result.Symbol, Is.EqualTo(_symbol));
                Assert.That(result.Type, Is.EqualTo(PumpAndDumpType.Pump));
                Assert.That(result.Price.Max, Is.EqualTo(prices[^1]));
                Assert.That(result.Price.Min, Is.EqualTo(prices[0]));
                Assert.That(result.Time.Max, Is.EqualTo((long)TimeSpan.FromMinutes(prices.Length).TotalMilliseconds));
                Assert.That(result.Time.Min, Is.EqualTo((long)TimeSpan.FromMinutes(1).TotalMilliseconds));
                
                Assert.That(result.Price.Min, Is.LessThan(result.Price.Max));
                Assert.That(result.Time.Min, Is.LessThan(result.Time.Max));
            });

            // Access private field using reflection
            FieldInfo? symbolBSTInfo = typeof(DataConsumerWorkerService).GetField("symbolBST", BindingFlags.NonPublic | BindingFlags.Instance);
            Dictionary<string, PriceBinarySearchTree>? value = symbolBSTInfo!.GetValue(_dataConsumerWorkerService) as Dictionary<string, PriceBinarySearchTree>;
            
            Assert.Multiple(() =>
            {
                Assert.That(value![_symbol].MinPrice.Value, Is.EqualTo(-1));
                Assert.That(value![_symbol].MaxPrice.Value, Is.EqualTo(-1));
            });
        }
    }
}