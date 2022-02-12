using System;

namespace BinanceBotApp.Data
{
    /// <summary>
    /// Authenticated user info
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// User id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// User login
        /// </summary>
        public string Login { get; set; }
        
        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// User surname
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// User roles' names
        /// </summary>
        public string RoleName { get; set; }
        
        /// <summary>
        /// User registration date
        /// </summary>
        public DateTime DateCreated { get; set; }
        
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; }
    }
}