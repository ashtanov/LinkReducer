using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LinkReducer.Models
{
    public class ReducerContext
    {
        private readonly IMongoDatabase _database = null;
        private const string COLLECTION_NAME = "UriEntities";
        public ReducerContext(Settings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Database);
        }
        public ReducerContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public void InitDatabase()
        {
            var fullUriIndex = Builders<UriEntity>.IndexKeys.Ascending(x => x.FullUri);
            var shortUriIndex = Builders<UriEntity>.IndexKeys.Ascending(x => x.ShortKey);
            var userIdIndex = Builders<UriEntity>.IndexKeys.Ascending(x => x.UserIds);
            UriEntities.Indexes.CreateOne(fullUriIndex);
            UriEntities.Indexes.CreateOne(shortUriIndex, new CreateIndexOptions { Unique = true });
            UriEntities.Indexes.CreateOne(userIdIndex);
        }

        public IMongoCollection<UriEntity> UriEntities
        {
            get
            {
                return _database.GetCollection<UriEntity>(COLLECTION_NAME);
            }
        }
    }
}
