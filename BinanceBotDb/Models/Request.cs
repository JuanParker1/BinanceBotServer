using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_request_log"), Comment("Application http request log")]
    public class Request
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("id_user")]
        public int IdUser { get; set; }
        
        [Column("login")]
        [StringLength(255)]
        public string Login { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }
        
        [Column("ip")]
        [StringLength(25)]
        public string Ip { get; set; }
        
        [Column("status")]
        public int Status { get; set; }
        
        [Column("request_method")]
        [StringLength(10)]
        public string RequestMethod { get; set; }
        
        [Column("request_path")]
        [StringLength(50)]
        public string RequestPath { get; set; }
        
        [Column("referer")]
        [StringLength(100)]
        public string Referer { get; set; }
        
        [Column("elapsed_ms")]
        public long ElapsedMilliseconds { get; set; }

        [Column("ex_message")]
        public string ExceptionMessage { get; set; }
        
        [Column("ex_stack")]
        public string ExceptionStack { get; set; }
        
        
        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(Models.User.RequestLog))]
        public virtual User User { get; set; }
    }
}