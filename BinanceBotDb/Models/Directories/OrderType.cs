using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models.Directories
{
    [Table("t_order_types"), Comment("Order types")]
    public class OrderType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("Caption"), Comment("Order type caption")]
        public string Caption { get; set; }
        
        
        [InverseProperty(nameof(Order.OrderType))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}