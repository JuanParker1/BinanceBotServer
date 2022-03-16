namespace BinanceBotApp.DataInternal.Deserializers
{
    /// <summary>
    /// Deleted order info object
    /// </summary>
    public class DeletedOrder : CreatedOrderResult
    {
        /// <summary>
        /// Cancelled order id
        /// </summary>
        public string OrigClientOrderId { get; set; }
    }
}