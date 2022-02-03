namespace BinanceBotApp.Data
{
    /// <summary>
    /// Base user info
    /// </summary>
    public class UserBaseDto
    {
        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; }
        
        /// <summary>
        /// User password
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// User surname
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }
    }
}