using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models.Directories
{
    [Table("t_trade_modes"), Comment("Trade modes")]
    public class TradeMode
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("caption"), Comment("Caption")]
        [StringLength(50)]
        public string Caption { get; set; }

        
        [InverseProperty(nameof(Models.Settings.TradeMode))]
        public virtual ICollection<Settings> Settings { get; set; }
    }
}