using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Bil
    {
        public int Id { get; set; }

        public bool ÄrAktiv { get; set; } = true;

        [Required(ErrorMessage = "Tillverkare saknas")]
        [MaxLength(100)]
        public string Tillverkare { get; set; } = null!;

        [Required(ErrorMessage = "Årsmodell saknas")]
        public int Årsmodell { get; set; }

        [Required(ErrorMessage = "Modell saknas")]
        [MaxLength(100)]
        public string Modell { get; set; } = null!;

        [Required(ErrorMessage = "drivning saknas")]
        [MaxLength(20)]
        public string Drivning { get; set; } = null!;

        [Required(ErrorMessage = "Bränsle saknas")]
        [MaxLength(20)]
        public string Bränsle { get; set; } = null!;

        [Required(ErrorMessage = "Växellåda saknas")]
        [MaxLength(20)]
        public string Växellåda { get; set; } = null!;

        [Required(ErrorMessage = "Beskrivning saknas")]
        public string Beskrivning { get; set; } = null!;

        [Required(ErrorMessage = "Bilder saknas")]
        public virtual List<string> Bilder { get; set; } = null!;
        public virtual List<Bokning>? Bokningar { get; set; } = null!;
    }
}
