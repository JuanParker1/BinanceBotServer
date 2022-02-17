using System.Collections.Generic;

namespace BinanceBotApp.Data
{
    /// <summary>
    /// Generic parameter for different types of request params
    /// </summary>
    public class GenericCollectionDto<T>
    {
        /// <summary>
        /// User id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// Generic collection of request params
        /// </summary>
        public IEnumerable<T> Collection { get; set; }
    }
}