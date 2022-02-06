using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_settings"), Comment("Application trade settings")]
    public class Settings
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_user")]
        public int IdUser { get; set; }

        [Column("is_trade_enabled"), Comment("Trade on/off")]
        public bool IsTradeEnabled { get; set; }
        
        [Column("trade_mode"), Comment("0 - trade only by signals \n 1 - purchase by signals, sale by limit order")]
        public int TradeMode { get; set; }
        
        [Column("limit_order_rate"), Comment("Amount of % from highest price to place limit order")]
        public int LimitOrderRate { get; set; }
        
        [Column("api_key"), Comment("api key")]
        [StringLength(100)]
        public string ApiKey { get; set; }
        
        [Column("secret_key"), Comment("secret key")]
        [StringLength(100)]
        public string SecretKey { get; set; }
        
        
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(Models.User.Settings))]
        public virtual User User { get; set; }
    }
}