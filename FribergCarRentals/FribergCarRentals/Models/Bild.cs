namespace FribergCarRentals.Models
{
    public class Bild
    {
        public int Id { get; set; }
        public string BildPath { get; set; } = null!;
        public Bil Bil { get; set; } = null!;
    }
}
