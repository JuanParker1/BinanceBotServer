using System;
using System.Collections;

namespace BinanceBotInfrastructure.Services.Cache
{
    public class CacheTableDataStorage
    {
        public string NameOfTEntity { get; set; }
        public DateTime LastRefreshDate { get; set; }
        public IEnumerable Entities { get; set; }
        public TimeSpan ObsolesenceTime  { get; set; } = TimeSpan.FromMinutes(15);
        public bool IsObsolete => (DateTime.Now - LastRefreshDate > ObsolesenceTime);
    }
}