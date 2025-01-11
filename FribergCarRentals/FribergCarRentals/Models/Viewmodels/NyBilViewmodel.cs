using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models.Viewmodels
{
    public class NyBilViewmodel
    {
        [Required(ErrorMessage = "tillverkare")]
        [MaxLength(100)]
        public string Tillverkare { get; set; } = null!;

        [Required(ErrorMessage = "årsmodell")]
        public int ÅrsModell { get; set; }

        [Required(ErrorMessage = "modell")]
        [MaxLength(100)]
        public string Modell { get; set; } = null!;

        [Required(ErrorMessage = "drivlina")]
        [MaxLength(5)]
        public string Drivlina { get; set; } = null!;

        [Required(ErrorMessage = "bränsle")]
        [MaxLength(20)]
        public string Bränsle { get; set; } = null!;

        [Required(ErrorMessage = "bränsleförbrukning")]
        public string BränsleFörbrukning { get; set; } = null!;

        [Required(ErrorMessage = "tankvolym")]
        public string Tankvolym { get; set; } = null!;

        [Required(ErrorMessage = "maxmotoreffekt")]
        public int MaxMotoreffekt { get; set; } // Hk

        [Required(ErrorMessage = "beskrivning")]
        public string Beskrivning { get; set; } = null!;

        [Required(ErrorMessage = "bilder")]
        public virtual List<string> Bilder { get; set; } = null!;
    }
}
