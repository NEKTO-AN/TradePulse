using System.ComponentModel;
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

        public async Task AddOrderbookItemAsync(AddOrderbookItemQuery query, CancellationToken cancellationToken = default)
        {
            if (query.OrderbookDataMessage.Data == null)
            {
                throw new Exception("Data model is empty");
            }

            Domain.Orderbook.Orderbook orderbook = new(query.OrderbookDataMessage.Timestamp, query.OrderbookDataMessage.Data);
            await _orderbookRepository.AddAsync(orderbook, cancellationToken);

            symbolBST[query.OrderbookDataMessage.Data.Symbol].Insert(query.OrderbookDataMessage.LastPrice, query.OrderbookDataMessage.Timestamp);
        }

        public async Task<Domain.PumpDumpSnapshot.PumpDumpSnapshot?> FindAnomalyAsync(FindAnomalyQuery query)
        {
            if (IsAnomalyNotFound(symbolBST[query.Symbol]))
            {
                return null;
            }

            long startAnomalyTs;
            long endAnomalyTs;
            PumpAndDumpType pumpAndDumpType;
            if (symbolBST[query.Symbol].MaxPrice.LastUpdateTs > symbolBST[query.Symbol].MinPrice.LastUpdateTs)
            {
                startAnomalyTs = symbolBST[query.Symbol].MinPrice.LastUpdateTs;
                endAnomalyTs = symbolBST[query.Symbol].MaxPrice.LastUpdateTs;
                pumpAndDumpType = PumpAndDumpType.Pump;
            }
            else
            {
                startAnomalyTs = symbolBST[query.Symbol].MaxPrice.LastUpdateTs;
                endAnomalyTs = symbolBST[query.Symbol].MinPrice.LastUpdateTs;
                pumpAndDumpType = PumpAndDumpType.Dump;
            }

            Domain.PumpDumpSnapshot.PumpDumpSnapshot snapshot = Domain.PumpDumpSnapshot.PumpDumpSnapshot.Create(
                symbol: query.Symbol,
                type: pumpAndDumpType,
                price: new(symbolBST[query.Symbol].MaxPrice.Value, symbolBST[query.Symbol].MinPrice.Value), 
                time: new(endAnomalyTs, startAnomalyTs));

            await _pumpDumpSnapshotRepository.AddAsync(snapshot);

            symbolBST[query.Symbol].ClearUntil(endAnomalyTs);

            return snapshot;
        }

        private static double CalculateRangePercent(double entryPrice, double exitPrice) => Math.Abs(entryPrice - exitPrice) / (entryPrice / 100);

        private static bool IsAnomalyNotFound(PriceBinarySearchTree priceBinarySearchTree)
            => (priceBinarySearchTree.MinPrice.LastUpdateTs < priceBinarySearchTree.MaxPrice.LastUpdateTs 
                && CalculateRangePercent(priceBinarySearchTree.MinPrice.Value, priceBinarySearchTree.MaxPrice.Value) < RANGE_PERCENT)
                || CalculateRangePercent(priceBinarySearchTree.MaxPrice.Value, priceBinarySearchTree.MinPrice.Value) < RANGE_PERCENT;
    }
}