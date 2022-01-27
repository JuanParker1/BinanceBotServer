using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    [Table("t_users"), Comment("Users")]
    public partial class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_role")]
        public int? IdRole { get; set; }

        [Column("login")]
        [StringLength(255)]
        public string Login { get; set; }

        [Column("password_hash"), Comment("Password hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Column("name"), Comment("Name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("surname"), Comment("Surname")]
        [StringLength(255)]
        public string Surname { get; set; }

        [Column("email"), Comment("Email")]
        [StringLength(255)]
        public string Email { get; set; }
        

        [ForeignKey(nameof(IdRole))]
        [InverseProperty(nameof(UserRole.Users))]
        public virtual UserRole Role { get; set; }
    }
}