using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models.Viewmodels
{
    public class NyBilViewmodel
    {
        [Required(ErrorMessage = "tillverkare")]
        [MaxLength(100)]
        public string Tillverkare { get; set; } = null!;

        [Required(ErrorMessage = "årsmodell")]
        public int Årsmodell { get; set; }

        [Required(ErrorMessage = "modell")]
        [MaxLength(100)]
        public string Modell { get; set; } = null!;

        [Required(ErrorMessage = "drivning")]
        [MaxLength(20)]
        public string Drivning { get; set; } = null!;

        [Required(ErrorMessage = "bränsle")]
        [MaxLength(20)]
        public string Bränsle { get; set; } = null!;

        [Required(ErrorMessage = "växellåda")]
        [MaxLength(20)]
        public string Växellåda { get; set; } = null!;

        [Required(ErrorMessage = "beskrivning")]
        public string Beskrivning { get; set; } = null!;

        [Required(ErrorMessage = "bilder")]
        public virtual List<string> Bilder { get; set; } = null!;
    }
}
