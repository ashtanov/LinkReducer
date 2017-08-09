using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            //добавить уникальный индекс на ключи, ловить исключения, возвращать ошибку
            var filter = Builders<UriEntity>.Filter.Eq(x => x.FullUri, fullUri);
            var update = Builders<UriEntity>
                .Update
                .AddToSet(x => x.UserIds, userId)
                .SetOnInsert(x => x.FullUri, fullUri)
                .SetOnInsert(x => x.Hits, 1)
                .SetOnInsert(x => x.ShortKey, shortKey);
            var options = new FindOneAndUpdateOptions<UriEntity, KeyObject>
            {
                Projection = Builders<UriEntity>.Projection.Expression(x => new KeyObject { ShortKey = x.ShortKey }),
                IsUpsert = true,
            };
            return _context.UriEntities.FindOneAndUpdate(filter, update, options);
        }
    }
}
