namespace BinanceBotApp.Data
{
    /// <summary>
    /// User password info
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Requested user id
        /// </summary>
        public int IdUser { get; set; }
        /// <summary>
        /// User old password
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// User new password
        /// </summary>
        public string NewPassword { get; set; }
    }
}