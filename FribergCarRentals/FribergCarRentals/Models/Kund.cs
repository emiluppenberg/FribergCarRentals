using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Kund
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

        [Required(ErrorMessage = "Förnamn saknas")]
        [MaxLength(100)]
        public string Förnamn { get; set; } = null!;

        [Required(ErrorMessage = "Efternamn saknas")]
        [MaxLength(100)]
        public string Efternamn { get; set; } = null!;

        [Required(ErrorMessage = "Telefonnummer saknas")]
        [MaxLength(20)]
        public string TelefonNummer { get; set; } = null!;

        [Required(ErrorMessage = "Adress saknas")]
        [MaxLength(100)]
        public string Adress { get; set; } = null!;

        [Required(ErrorMessage = "Postkod saknas")]
        [MaxLength(20)]
        public string Postkod { get; set; } = null!;

        [Required(ErrorMessage = "Ort saknas")]
        [MaxLength(20)]
        public string Ort { get; set; } = null!;

        public virtual List<Bokning> Bokningar { get; set; } = new();
    }
}
