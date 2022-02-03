namespace BinanceBotApp.Data
{
    /// <summary>
    /// New user register info
    /// </summary>
    public class RegisterDto : UserBaseDto
    {
        /// <summary>
        /// New user role id
        /// </summary>
        public int? IdRole { get; set; }
        
        /// <summary>
        /// New user password
        /// </summary>
        public string Password { get; set; }
    }
}