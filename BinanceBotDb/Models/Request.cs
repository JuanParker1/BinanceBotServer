using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_request_log"), Comment("Application http requests log")]
    public class Request
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("id_user")]
        public int IdUser { get; set; }
        
        [Column("login")]
        public string Login { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }
        
        [Column("ip")]
        public string Ip { get; set; }
        
        [Column("status")]
        public int Status { get; set; }
        
        [Column("request_method")]
        public string RequestMethod { get; set; }
        
        [Column("request_path")]
        public string RequestPath { get; set; }
        
        [Column("referer")]
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