namespace BinanceBotApp.Data
{
    /// <summary>
    /// New user register info
    /// </summary>
    public class RegisterDto : UserBaseDto
    {
        public int Id { get; set; }
        public int? IdRole { get; set; }
        public string Password { get; set; }
    }
}