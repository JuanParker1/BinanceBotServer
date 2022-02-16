using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_events"), Comment("User/application event log")]
    public class Event
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("id_user")]
        public int IdUser { get; set; }
        
        [Column("date"), Comment("Дата совершения события")]
        public DateTime Date { get; set; }
        
        [Column("is_read"), Comment("Shows if event was read by user or not")]
        public bool IsRead { get; set; }
        
        [Column("text"), Comment("Event text")]
        [StringLength(700)]
        public string Text { get; set; }
        
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(Models.User.EventLog))]
        public virtual User User { get; set; }
    }
}