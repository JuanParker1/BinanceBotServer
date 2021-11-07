using System.Collections.Generic;

namespace BinanceBotApp.Data
{
    public class GenericCollectionDto<T>
    {
        public IEnumerable<T> Collection { get; set; }
    }
}