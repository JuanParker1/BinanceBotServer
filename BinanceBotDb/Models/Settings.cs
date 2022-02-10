using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BinanceBotDb.Models.Directories;
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
        
        [Column("id_trade_mode"), Comment("Selected trade mode")]
        public int IdTradeMode { get; set; }
        
        [Column("limit_order_rate"), Comment("Amount of percents from highest price to place limit order")]
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
        
        [ForeignKey(nameof(IdTradeMode))]
        [InverseProperty(nameof(Models.Directories.TradeMode.Settings))]
        public virtual TradeMode TradeMode { get; set; }
    }
}