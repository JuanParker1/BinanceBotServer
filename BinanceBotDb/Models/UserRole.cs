using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_user_role"), Comment("Роли пользователей")]
    public class UserRole
    {
        public UserRole()
        {
            Users = new HashSet<User>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("caption"), Comment("Название")]
        [StringLength(255)]
        public string Caption { get; set; }

        [InverseProperty(nameof(User.Role))]
        public virtual ICollection<User> Users { get; set; }
    }
}