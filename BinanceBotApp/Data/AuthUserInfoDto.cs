namespace BinanceBotApp.Data
{
    /// <summary>
    /// Authenticated user info
    /// </summary>
    public class AuthUserInfoDto : UserBaseDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Token { get; set; }
        public bool isApiKeysSet { get; set; }
    }
}