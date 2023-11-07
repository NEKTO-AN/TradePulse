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

        public async Task AddAsync(Orderbook entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public Task<List<Orderbook>> GetAsync(string symbol, long fromTs, int count)
            => _collection.AsQueryable()
                .Where(x => x.Timestamp > fromTs)
                .Take(count)
                .ToListAsync();
    }
}

