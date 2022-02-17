using System;

namespace BinanceBotApp.Data
{
    /// <summary>
    /// User request info
    /// </summary>
    public class RequestDto
    {
        /// <summary>
        /// User id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; } // TODO: Описание ВСЕХ Dto
        
        /// <summary>
        /// Request date
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// User ip
        /// </summary>
        public string Ip { get; set; }
        
        /// <summary>
        /// Http response status
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// Http request method
        /// </summary>
        public string RequestMethod { get; set; }
        
        /// <summary>
        /// Request path
        /// </summary>
        public string RequestPath { get; set; }
        
        /// <summary>
        /// Request headers Referer value
        /// </summary>
        public string Referer { get; set; }
        
        /// <summary>
        /// Request duration
        /// </summary>
        public long ElapsedMilliseconds { get; set; }
    }
}