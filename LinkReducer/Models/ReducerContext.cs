using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkReducer.Models
{
    public class ReducerContext
    {
        private readonly IMongoDatabase _database = null;

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

        public IMongoCollection<UriEntity> UriEntities
        {
            get
            {
                return _database.GetCollection<UriEntity>("UriEntities");
            }
        }
    }
}
