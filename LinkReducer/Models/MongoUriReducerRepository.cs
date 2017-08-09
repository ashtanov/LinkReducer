using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkReducer.Models
{
    public class MongoUriReducerRepository : IUriReducerRepository
    {
        private readonly ReducerContext _context;
        public MongoUriReducerRepository(IOptions<Settings> settings)
        {
            _context = new ReducerContext(settings);
        }

        public string GetFullUri(string shortKey)
        {
            var filter = Builders<UriEntity>.Filter.Eq(x => x.ShortKey, shortKey);
            var update = Builders<UriEntity>.Update.Inc(x => x.Hits, 1);
            var options = new FindOneAndUpdateOptions<UriEntity, string>
            {
                Projection = Builders<UriEntity>.Projection.Expression(x => x.FullUri)
            };
            return _context.UriEntities.FindOneAndUpdate(filter, update, options);
        }

        public IEnumerable<UriStat> GetHistoryList(Guid userId)
        {
            var filter = Builders<UriEntity>.Filter.AnyEq(x => x.UserIds, userId);
            var projection = Builders<UriEntity>.Projection.Exclude(x => x.UserIds).Exclude(x => x.Id);
            var elems = _context.UriEntities.Find(filter).Project<UriStat>(projection).ToList();
            return elems;
        }

        public KeyObject GetOrInsertShortKey(string fullUri, Guid userId, string shortKey)
        {
            KeyObject result = new KeyObject { Success = true };
            try
            {
                var filter = Builders<UriEntity>.Filter.Eq(x => x.FullUri, fullUri);
                var update = Builders<UriEntity>
                    .Update
                    .AddToSet(x => x.UserIds, userId)
                    .Inc(x => x.Hits, 1)
                    .SetOnInsert(x => x.FullUri, fullUri)
                    .SetOnInsert(x => x.ShortKey, shortKey);
                var options = new FindOneAndUpdateOptions<UriEntity, string>
                {
                    Projection = Builders<UriEntity>.Projection.Expression(x => x.ShortKey),
                    IsUpsert = true
                };
                var resultKey = _context.UriEntities.FindOneAndUpdate(filter, update, options) ?? shortKey;
                result.ShortKey = resultKey;
            }
            catch (MongoCommandException ex) when (ex.Code == 11000)
            {
                result.Success = false;
            }
            return result;

        }

        public void Init()
        {
            _context.InitDatabase();
        }
    }
}
