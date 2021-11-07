namespace BinanceBotApp.Data
{
    /// <summary>
    /// User info
    /// </summary>
    public class UserDto : UserBaseDto
    {
        public int Id { get; set; }
        public int? IdRole { get; set; }
        public string Password { get; set; }
    }
}