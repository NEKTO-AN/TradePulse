using Domain.Orderbook;
using MongoDB.Driver;

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
    }
}

