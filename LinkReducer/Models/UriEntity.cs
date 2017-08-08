using System;
using MongoDB.Bson;

namespace LinkReducer.Models
{
    public class UriEntity
    {
        public ObjectId Id { get; set; }
        public Guid UserId { get; set; }
        public string FullUri { get; set; }
        public string ShortKey { get; set; }
    }
}
