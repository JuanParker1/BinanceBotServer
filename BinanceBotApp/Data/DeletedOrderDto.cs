namespace BinanceBotApp.Data
{
    public class DeletedOrderDto : CreatedOrderResultDto
    {
        public string OrigClientOrderId { get; set; }
    }
}