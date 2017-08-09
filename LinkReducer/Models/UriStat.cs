using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkReducer.Models
{
    public class UriStat
    {
        public string FullUri { get; set; }
        public string ShortKey { get; set; }
        public int Hits { get; set; }
    }
}
