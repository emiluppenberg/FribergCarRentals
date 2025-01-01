namespace FribergCarRentals.Models
{
    public class Kund
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Lösenord { get; set; } = null!;
        public string Förnamn { get; set; } = null!;
        public string Efternamn { get; set; } = null!;
        public string TelefonNummer { get; set; } = null!;
        public string Adress { get; set; } = null!;
        public string Postkod { get; set; } = null!;
        public string Ort { get; set; } = null!;
        public virtual List<Bokning> Bokningar { get; set; } = new();
    }
}
