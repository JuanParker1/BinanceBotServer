namespace BinanceBotApp.Data
{
    public class UserDto : UserBaseDto
    {
        public int Id { get; set; }
        public int? IdRole { get; set; }
        public string Password { get; set; }
    }
}