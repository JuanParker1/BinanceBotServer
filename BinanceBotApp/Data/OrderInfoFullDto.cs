using System.Collections.Generic;

namespace BinanceBotApp.Data
{
    public class OrderInfoFullDto : OrderInfoResultDto
    {
        public IEnumerable<OrderFillPartDto> FillParts { get; set; }
    }
}