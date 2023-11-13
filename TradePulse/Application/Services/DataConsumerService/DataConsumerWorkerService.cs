using Application.Helpers.Configuration;
using Domain.Orderbook;
using Domain.PumpDumpSnapshot;

namespace Application.Services.DataConsumerService
{
    public class DataConsumerWorkerService
    {
        private const float RANGE_PERCENT = .5f;    //
        private readonly TimeSpan SYMBOL_PRICE_LIFETIME = TimeSpan.FromHours(1);
        private readonly Dictionary<string, PriceBinarySearchTree> symbolBST = new();

        private readonly IOrderbookRepository _orderbookRepository;
        private readonly IPumpDumpSnapshotRepository _pumpDumpSnapshotRepository;

        public DataConsumerWorkerService(IOrderbookRepository orderbookRepository, IPumpDumpSnapshotRepository pumpDumpSnapshotRepository, AppConfiguration appConfiguration)
        {
            _orderbookRepository = orderbookRepository;
            _pumpDumpSnapshotRepository = pumpDumpSnapshotRepository;
            symbolBST = appConfiguration.Symbols.ToDictionary(k => k, v => new PriceBinarySearchTree(SYMBOL_PRICE_LIFETIME));
        }

        public Task AddOrderbookItemAsync(Domain.Orderbook.Orderbook orderbook, CancellationToken cancellationToken = default) 
            => _orderbookRepository.AddAsync(orderbook, cancellationToken);

        public async Task FindAnomalyAsync(string symbol, long timestamp, double lastPrice)
        {
            symbolBST[symbol].Insert(lastPrice, timestamp);

            if (IsAnomalyFound(symbolBST[symbol]))
            {
                return;
            }

            long startAnomalyTs;
            long endAnomalyTs;
            PumpAndDumpType pumpAndDumpType;
            if (symbolBST[symbol].MaxPrice.LastUpdateTs > symbolBST[symbol].MinPrice.LastUpdateTs)
            {
                startAnomalyTs = symbolBST[symbol].MinPrice.LastUpdateTs;
                endAnomalyTs = symbolBST[symbol].MaxPrice.LastUpdateTs;
                pumpAndDumpType = PumpAndDumpType.Pump;
            }
            else
            {
                startAnomalyTs = symbolBST[symbol].MaxPrice.LastUpdateTs;
                endAnomalyTs = symbolBST[symbol].MinPrice.LastUpdateTs;
                pumpAndDumpType = PumpAndDumpType.Dump;
            }

            await _pumpDumpSnapshotRepository.AddAsync(PumpDumpSnapshot.Create(
                type: pumpAndDumpType, 
                price: new(symbolBST[symbol].MaxPrice.Value, symbolBST[symbol].MinPrice.Value), 
                time: new(endAnomalyTs, startAnomalyTs)));

            symbolBST[symbol].ClearUntil(endAnomalyTs);
        }

        private static double CalculateRangePercent(double entryPrice, double exitPrice) => Math.Abs(entryPrice - exitPrice) / (entryPrice / 100);

        private static bool IsAnomalyFound(PriceBinarySearchTree priceBinarySearchTree)
            => (priceBinarySearchTree.MinPrice.LastUpdateTs < priceBinarySearchTree.MaxPrice.LastUpdateTs 
                && CalculateRangePercent(priceBinarySearchTree.MinPrice.Value, priceBinarySearchTree.MaxPrice.Value) < RANGE_PERCENT)
                || CalculateRangePercent(priceBinarySearchTree.MaxPrice.Value, priceBinarySearchTree.MinPrice.Value) < RANGE_PERCENT;
    }
}