using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Admin
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
    }
}
