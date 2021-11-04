namespace BinanceBotApp.Data
{
    public class UserTokenDto : UserBaseDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Token { get; set; }
    }
}