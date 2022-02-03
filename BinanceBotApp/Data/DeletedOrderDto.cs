namespace BinanceBotApp.Data
{
    /// <summary>
    /// Deleted order info object
    /// </summary>
    public class DeletedOrderDto : CreatedOrderResultDto
    {
        /// <summary>
        /// Cancelled order id
        /// </summary
        public string OrigClientOrderId { get; set; }
    }
}