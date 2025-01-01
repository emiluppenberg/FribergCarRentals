namespace FribergCarRentals.Models
{
    public class Bil
    {
        public int Id { get; set; }
        public string Tillverkare { get; set; } = null!;
        public int TillverkningsÅr { get; set; }
        public string Modell { get; set; } = null!;
        public string Drivlina { get; set; } = null!;
        public string Bränsle { get; set; } = null!;
        public int BränsleFörbrukning { get; set; } // Per 100km
        public int Tankvolym { get; set; }
        public int MaxMotoreffekt { get; set; } // Hk
        public string Beskrivning { get; set; } = null!;
        public virtual List<Bild> Bilder { get; set; } = new();
    }
}
