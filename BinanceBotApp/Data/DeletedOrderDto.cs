namespace BinanceBotApp.Data
{
    /// <summary>
    /// Deleted order info object
    /// </summary>
    public class DeletedOrderDto : CreatedOrderResultDto
    {
        public string OrigClientOrderId { get; set; }
    }
}