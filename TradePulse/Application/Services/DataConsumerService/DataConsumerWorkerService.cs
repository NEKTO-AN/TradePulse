using Application.Helpers.Configuration;
using Domain.Orderbook;

namespace Application.Services.DataConsumerService
{
    public class DataConsumerWorkerService
    {
        private readonly Dictionary<string, PriceBinarySearchTree> symbolBST = new();

        private readonly IOrderbookRepository _orderbookRepository;

        public DataConsumerWorkerService(IOrderbookRepository orderbookRepository, AppConfiguration appConfiguration)
        {
            _orderbookRepository = orderbookRepository;
            foreach (string s in appConfiguration.OrderbookTopics)
            {
                symbolBST.Add(s.Split('.').Last(), new(TimeSpan.FromHours(1)));
            }
        }

        public async Task AddOrderbookItemAsync(Domain.Orderbook.Orderbook orderbook)
        {
            await _orderbookRepository.AddAsync(orderbook);
            
            if (orderbook.Data.Asks.Length > 0)
            {
                symbolBST[orderbook.Symbol].Insert(orderbook.Data.Asks[0][0], orderbook.Timestamp);
            }
            else if (orderbook.Data.Bids.Length > 0)
            {
                symbolBST[orderbook.Symbol].Insert(orderbook.Data.Bids[0][0], orderbook.Timestamp);
            }

            if (CalculateRangePercent(symbolBST[orderbook.Symbol].MinPrice, symbolBST[orderbook.Symbol].MaxPrice) < 2)
            {
                return;
            }

            
        }

        private static double CalculateRangePercent(double entryPrice, double exitPrice) => Math.Abs(entryPrice - exitPrice) / (entryPrice / 100);
    }
}