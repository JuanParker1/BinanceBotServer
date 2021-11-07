namespace BinanceBotApp.Data
{
    /// <summary>
    /// User JWT authentication token
    /// </summary>
    public class UserTokenDto : UserBaseDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Token { get; set; }
    }
}