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
        
        [Column("id_user"), Comment("Request user id")]
        public int IdUser { get; set; }
        
        [Column("login"), Comment("Request user login")]
        [StringLength(255)]
        public string Login { get; set; }

        [Column("date"), Comment("Request date")]
        public DateTime Date { get; set; }
        
        [Column("ip"), Comment("Request ip address")]
        [StringLength(25)]
        public string Ip { get; set; }
        
        [Column("status"), Comment("Request http status")]
        public int Status { get; set; }
        
        [Column("request_method"), Comment("Request http method")]
        [StringLength(10)]
        public string RequestMethod { get; set; }
        
        [Column("request_path"), Comment("Request path")]
        [StringLength(50)]
        public string RequestPath { get; set; }
        
        [Column("referer"), Comment("Request referer")]
        [StringLength(100)]
        public string Referer { get; set; }
        
        [Column("elapsed_ms"), Comment("Request lifetime")]
        public long ElapsedMilliseconds { get; set; }


        [ForeignKey(nameof(IdUser))]
        [InverseProperty(nameof(Models.User.RequestLog))]
        public virtual User User { get; set; }
    }
}