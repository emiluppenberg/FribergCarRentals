using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Kund : User
    {
        [Required(ErrorMessage = "Förnamn saknas")]
        [MaxLength(100)]
        public string Förnamn { get; set; } = null!;

        [Required(ErrorMessage = "Efternamn saknas")]
        [MaxLength(100)]
        public string Efternamn { get; set; } = null!;

        [Phone]
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
        public Kund()
        {
            this.Role = "kund";
        }
    }
}
