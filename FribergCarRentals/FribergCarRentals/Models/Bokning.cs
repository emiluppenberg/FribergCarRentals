namespace FribergCarRentals.Models
{
    public class Bokning
    {
        public int Id { get; set; }
        public DateTime Startdatum { get; set; }
        public DateTime Slutdatum { get; set; }
        public Kund Kund { get; set; } = null!;
        public Bil Bil { get; set; } = null!;
    }
}
