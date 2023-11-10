using Domain.PumpDumpSnapshot;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class PumpDumpSnapshotRepository : IPumpDumpSnapshotRepository
    {
		private readonly IMongoCollection<PumpDumpSnapshot> _collection;
        
        public PumpDumpSnapshotRepository(MongoDbContext dbContext)
		{
			dbContext.Database.CreateCollection(nameof(PumpDumpSnapshot));
            _collection = dbContext.Database.GetCollection<PumpDumpSnapshot>(nameof(PumpDumpSnapshot));
		}

        public async Task AddAsync(PumpDumpSnapshot entity)
        {
            await _collection.InsertOneAsync(entity);
        }
    }
}