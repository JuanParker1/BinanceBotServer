using System.Collections.Generic;

namespace BinanceBotApp.Data
{
    public class CreatedOrderFullDto : CreatedOrderResultDto
    {
        public IEnumerable<OrderFillPartDto> FillParts { get; set; }
    }
}