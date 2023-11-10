using Application.Helpers.Configuration;
using Domain.Orderbook;
using Domain.PumpDumpSnapshot;

namespace Application.Services.DataConsumerService
{
    public class DataConsumerWorkerService
    {
        private readonly Dictionary<string, PriceBinarySearchTree> symbolBST = new();
        private readonly Dictionary<string, long> lastFundedAnomaly = new();    //anomaly is pump or dump

        private readonly IOrderbookRepository _orderbookRepository;
        private readonly IPumpDumpSnapshotRepository _pumpDumpSnapshotRepository;

        public DataConsumerWorkerService(IOrderbookRepository orderbookRepository, IPumpDumpSnapshotRepository pumpDumpSnapshotRepository, AppConfiguration appConfiguration)
        {
            _orderbookRepository = orderbookRepository;
            _pumpDumpSnapshotRepository = pumpDumpSnapshotRepository;
            foreach (string s in appConfiguration.Symbols)
            {
                symbolBST.Add(s, new(TimeSpan.FromHours(1)));
                lastFundedAnomaly.Add(s, 0);
            }
        }

        public async Task AddOrderbookItemAsync(Domain.Orderbook.Orderbook orderbook, double lastPrice)
        {
            await _orderbookRepository.AddAsync(orderbook);
            
            if (orderbook.Data.Bids.Length < 1 || orderbook.Data.Asks.Length < 1)
            {
                return;
            }

            PriceBinarySearchTree priceBinarySearchTree = symbolBST[orderbook.Symbol];
            
            symbolBST[orderbook.Symbol].Insert(lastPrice, orderbook.Timestamp);

            if (lastFundedAnomaly[orderbook.Symbol] > orderbook.Timestamp - 3600_000
                || (priceBinarySearchTree.MinPrice.LastUpdateTs < priceBinarySearchTree.MaxPrice.LastUpdateTs 
                && CalculateRangePercent(priceBinarySearchTree.MinPrice.Value, priceBinarySearchTree.MaxPrice.Value) < 2)
                || CalculateRangePercent(priceBinarySearchTree.MaxPrice.Value, priceBinarySearchTree.MinPrice.Value) < 2)
            {
                return;
            }

            PumpAndDumpType pumpAndDumpType = priceBinarySearchTree.MaxPrice.LastUpdateTs > priceBinarySearchTree.MinPrice.LastUpdateTs 
                ? PumpAndDumpType.Pump 
                : PumpAndDumpType.Dump;

            await _pumpDumpSnapshotRepository.AddAsync(PumpDumpSnapshot.Create(
                type: pumpAndDumpType, 
                price: new(priceBinarySearchTree.MaxPrice.Value, priceBinarySearchTree.MinPrice.Value), 
                time: new(priceBinarySearchTree.MaxPrice.LastUpdateTs, priceBinarySearchTree.MaxPrice.LastUpdateTs)));

            lastFundedAnomaly[orderbook.Symbol] = priceBinarySearchTree.MaxPrice.LastUpdateTs > priceBinarySearchTree.MinPrice.LastUpdateTs 
                ? priceBinarySearchTree.MaxPrice.LastUpdateTs 
                : priceBinarySearchTree.MinPrice.LastUpdateTs;
        }

        private static double CalculateRangePercent(double entryPrice, double exitPrice) => Math.Abs(entryPrice - exitPrice) / (entryPrice / 100);
    }
}