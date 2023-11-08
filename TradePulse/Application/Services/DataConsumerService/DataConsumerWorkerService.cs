using Domain.Orderbook;

namespace Application.Services.DataConsumerService
{
    public class DataConsumerWorkerService
    {
        private readonly PriceBinarySearchTree priceBinarySearchTree = new();

        private readonly IOrderbookRepository _orderbookRepository;

        public DataConsumerWorkerService(IOrderbookRepository orderbookRepository)
        {
            _orderbookRepository = orderbookRepository;
        }

        public async Task AddOrderbookItemAsync(Domain.Orderbook.Orderbook orderbook)
        {
            await _orderbookRepository.AddAsync(orderbook);
            
            if (orderbook.Data.Asks.Length > 0)
            {
                priceBinarySearchTree.Insert(orderbook.Data.Asks[0][0], orderbook.Timestamp);
            }
            else if (orderbook.Data.Bids.Length > 0)
            {
                priceBinarySearchTree.Insert(orderbook.Data.Bids[0][0], orderbook.Timestamp);
            }
        }
    }
}