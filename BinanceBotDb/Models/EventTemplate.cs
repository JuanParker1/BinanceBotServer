using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_event_templates"), Comment("Event log text templates")]
    public class EventTemplate
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("template"), Comment("Template text")]
        [StringLength(500)]
        public string Template { get; set; }
    }
}