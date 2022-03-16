using System.Collections.Generic;

namespace BinanceBotApp.DataInternal.Deserializers
{
    /// <summary>
    /// The most full new order info object
    /// </summary>
    public class CreatedOrderFull : CreatedOrderResult
    {
        /// <summary>
        /// List of successful orders, that filled that order
        /// </summary>
        public IEnumerable<OrderFillPart> FillParts { get; set; }
    }
}