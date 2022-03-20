using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BinanceBotDb.Models.Directories;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_orders"), Comment("Trade orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("client_order_id"), Comment("Exchange inner id")]
        public string ClientOrderId { get; set; }
        
        [Column("order_id"), Comment("Exchange inner id")]
        public long OrderId { get; set; }
        
        [Column("id_user")]
        public int IdUser { get; set; }
        
        [Column("symbol"), Comment("Trade pair")]
        public string Symbol { get; set; }
        
        [Column("date_created"), Comment("Order creation date")]
        public DateTime DateCreated { get; set; }
        
        [Column("date_closed"), Comment("Order closing date")]
        public DateTime? DateClosed { get; set; }
        
        [Column("id_side"), Comment("1 - Buy\n2 - Sell")]
        public int IdSide { get; set; }
        
        [Column("id_type"), Comment("1 - Limit\n2 - Market")]
        public int IdType { get; set; }
        
        [Column("time_in_force"), Comment("GTC, IOC, FOK")]
        public string TimeInForce { get; set; }
        
        [Column("quantity"), Comment("Amount of base asset")]
        public double Quantity { get; set; }
        
        [Column("quote_order_qty"), Comment("Amount of secondary asset")]
        public double QuoteOrderQty { get; set; }
        
        [Column("price"), Comment("Order price in secondary asset of trading pair")]
        public double Price { get; set; }

        [Column("id_creation_type"), Comment("1 - Auto\n2 - Manual")]
        public int IdCreationType { get; set; }


        [ForeignKey(nameof(IdType))]
        [InverseProperty(nameof(Models.Directories.OrderType.Orders))]
        public virtual OrderType OrderType { get; set; }
    }
}