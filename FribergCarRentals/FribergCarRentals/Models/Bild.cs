using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Bild
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bild saknas")]
        public string BildPath { get; set; } = null!;

        public Bil Bil { get; set; } = null!;
    }
}
