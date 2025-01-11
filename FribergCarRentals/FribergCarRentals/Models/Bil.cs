using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    public class Bil
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tillverkare saknas")]
        [MaxLength(100)]
        public string Tillverkare { get; set; } = null!;

        [Required(ErrorMessage = "Tillverkningsår saknas")]
        public int ÅrsModell { get; set; }

        [Required(ErrorMessage = "Modell saknas")]
        [MaxLength(100)]
        public string Modell { get; set; } = null!;

        [Required(ErrorMessage = "Drivlina saknas")]
        [MaxLength(5)]
        public string Drivlina { get; set; } = null!;

        [Required(ErrorMessage = "Bränsle saknas")]
        [MaxLength(20)]
        public string Bränsle { get; set; } = null!;

        [Required(ErrorMessage = "Bränsle förbrukning saknas")]
        public string BränsleFörbrukning { get; set; } = null!;

        [Required(ErrorMessage = "Tankvolym saknas")]
        public string Tankvolym { get; set; } = null!;

        [Required(ErrorMessage = "Max motoreffekt saknas")]
        public int MaxMotoreffekt { get; set; } // Hk

        [Required(ErrorMessage = "Beskrivning saknas")]
        public string Beskrivning { get; set; } = null!;

        [Required(ErrorMessage = "Bilder saknas")]
        public virtual List<string> Bilder { get; set; } = new();
    }
}
