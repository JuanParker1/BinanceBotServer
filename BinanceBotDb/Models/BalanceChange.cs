using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_balance_changes"), Comment("User balance deposits and withdrawals")]
    public class BalanceChange : IId
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("id_user")]
        public int IdUser { get; set; }
        
        [Column("date"), Comment("Balance change date")]
        public DateTime Date { get; set; }
        
        [Column("id_direction"), Comment("1 - Deposit \n2 - Withdraw")]
        public int IdDirection { get; set; }
        
        [Column("coin"), Comment("Coin name")]
        public string Coin { get; set; }
        
        [Column("amount"), Comment("Amount of change in USDT")]
        public double Amount { get; set; }
        
        
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(Models.User.BalanceChanges))]
        public virtual User User { get; set; }
    }
}