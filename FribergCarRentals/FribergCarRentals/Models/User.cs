using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergCarRentals.Models
{
    public abstract class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Email saknas")]
        [EmailAddress(ErrorMessage = "Email ogiltigt")]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Lösenord saknas")]
        [MinLength(5, ErrorMessage = "Minst 5 karaktärer")]
        [MaxLength(100)]
        public string Lösenord { get; set; } = null!;

        [NotMapped]
        public virtual string Role { get; set; } = null!;
    }
}