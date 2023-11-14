using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure
{
    public class MongoDbContext
	{
		private readonly IConfiguration _configuration;
        internal readonly IMongoDatabase Database;

        public MongoDbContext(IConfiguration configuration)
        {
			_configuration = configuration;

            MongoClient client = new(_configuration["MONGODB_URI"]);
            Database = client.GetDatabase("TradePulse");
        }
	}
}

