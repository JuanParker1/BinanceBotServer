using System.Collections.Generic;

namespace BinanceBotApp.Data
{
    /// <summary>
    /// The most full new order info object
    /// </summary>
    public class CreatedOrderFullDto : CreatedOrderResultDto
    {
        /// <summary>
        /// List of successful orders, that filled that order
        /// </summary>
        public IEnumerable<OrderFillPartDto> FillParts { get; set; }
    }
}