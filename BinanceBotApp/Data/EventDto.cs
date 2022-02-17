using System;

namespace BinanceBotApp.Data
{
    /// <summary>
    /// Application/User event info
    /// </summary>
    public class EventDto : IId
    {
        /// <summary>
        /// Event id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// User id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// Event creation date
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Did user read event's text or not
        /// </summary>
        public bool IsRead { get; set; }
        
        /// <summary>
        /// Event description
        /// </summary>
        public string Text { get; set; }
    }
}