using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkReducer.Models
{
    public class UriEntity
    {
        public Guid UserId { get; set; }
        public string FullUri { get; set; }
        public string ShortKey { get; set; }
    }
}
