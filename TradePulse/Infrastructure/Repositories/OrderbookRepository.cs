using Domain.Orderbook;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infrastructure.Repositories
{
    public class OrderbookRepository : IOrderbookRepository
    {
		private readonly IMongoCollection<Orderbook> _collection;


        public OrderbookRepository(MongoDbContext dbContext)
		{
			dbContext.Database.CreateCollection(nameof(Orderbook));
            _collection = dbContext.Database.GetCollection<Orderbook>(nameof(Orderbook));
		}

        public Task AddAsync(Orderbook entity, CancellationToken cancellationToken = default) 
            => _collection.InsertOneAsync(entity, null, cancellationToken);

        public Task<List<Orderbook>> GetAsync(string symbol, long fromTs, int count, CancellationToken cancellationToken = default)
            => _collection.AsQueryable()
                .Where(x => x.Timestamp > fromTs)
                .Take(count)
                .ToListAsync(cancellationToken);
    }
}

