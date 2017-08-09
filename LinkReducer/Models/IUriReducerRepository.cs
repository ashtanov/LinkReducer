using System;
using System.Collections.Generic;

namespace LinkReducer.Models
{
    public interface IUriReducerRepository
    {
        string GetFullUri(string shortKey);
        KeyObject GetOrInsertShortKey(string fullUri, Guid userId, string shortKey);
        IEnumerable<UriStat> GetHistoryList(Guid userId);
        void Init();
    }
}
