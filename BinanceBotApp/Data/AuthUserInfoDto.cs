namespace BinanceBotApp.Data
{
    /// <summary>
    /// Authenticated user info
    /// </summary>
    public class AuthUserInfoDto : UserBaseDto
    {
        /// <summary>
        /// User roles' names
        /// </summary>
        public string RoleName { get; set; }
        
        /// <summary>
        /// User token
        /// </summary>
        public string Token { get; set; }
    }
}