using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace LinkReducer.Models
{
    public class UriEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public List<Guid> UserIds { get; set; }
        public string FullUri { get; set; }
        public string ShortKey { get; set; }
        public int Hits { get; set; }
    }
}
