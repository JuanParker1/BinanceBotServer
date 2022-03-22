using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models.Directories
{
    [Table("t_order_status"), Comment("Order status")]
    public class OrderStatus
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("Caption"), Comment("Order status caption")]
        public string Caption { get; set; }

        [InverseProperty(nameof(Order.OrderStatus))]
        public virtual ICollection<Order> Orders { get; set; }
    }
}