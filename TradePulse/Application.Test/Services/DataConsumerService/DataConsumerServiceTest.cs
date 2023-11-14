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
        private readonly Mock<IOrderbookRepository> _orderbookRepositoryMock = new();
        private readonly Mock<IPumpDumpSnapshotRepository> _pumpDumpSnapshotRepositoryMock = new();
        private readonly Mock<AppConfiguration> _appConfigurationMock;

        private readonly DataConsumerWorkerService _dataConsumerWorkerService;
        
        public DataConsumerServiceTest()
        {
            Mock<IConfigurationSection> _symbolsConfigurationSection = new();
            _symbolsConfigurationSection.Setup(x => x.Value).Returns("BTCUSDT");
            Mock<IConfigurationSection> _topicsConfigurationSection = new();
            _topicsConfigurationSection.Setup(x => x.Value).Returns("orderbook.500");

            Mock<IConfiguration> _configuration = new();
            _configuration.Setup(x => x["KAFKA_ORDERBOOK_TOPIC"]).Returns("some-topic");
            _configuration.Setup(x => x["KAFKA_ENDPOINT"]).Returns("some-endpoint");
            _configuration.Setup(x => x.GetSection("Symbols")).Returns(_symbolsConfigurationSection.Object);
            _configuration.Setup(x => x.GetSection("Topics")).Returns(_topicsConfigurationSection.Object);

            _appConfigurationMock = new(_configuration.Object);

            _dataConsumerWorkerService = new(_orderbookRepositoryMock.Object, _pumpDumpSnapshotRepositoryMock.Object, _appConfigurationMock.Object);
        }

        [Test]
        public async Task AddOrderbookItemAsync_Positive_Test()
        {
            Domain.Orderbook.Orderbook orderbookEntity = new(
                timestamp: 0,
                symbol: "BTCUSDT",
                data: new(
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
                    crossSequence: 1
                )
            );
            _orderbookRepositoryMock.Setup(x => x.AddAsync(orderbookEntity, default))
                .Returns(Task.CompletedTask);

            await _dataConsumerWorkerService.AddOrderbookItemAsync(orderbookEntity);

            Assert.Pass();
        }
    }
}