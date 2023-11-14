using Domain.PumpDumpSnapshot;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

        public Task AddAsync(PumpDumpSnapshot entity, CancellationToken cancellationToken = default) 
            => _collection.InsertOneAsync(entity, null, cancellationToken);

        public Task<List<PumpDumpSnapshot>> GetAsync(string symbol, long fromTs, int count, CancellationToken cancellationToken = default)
        {
            return _collection.AsQueryable()
                .Where(x => x.Symbol == symbol && x.Time.Min > fromTs)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}