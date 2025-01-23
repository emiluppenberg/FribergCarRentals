using FribergCarRentals.Models;

namespace FribergCarRentals.Helpers
{
    public static class BokningarHelper
    {
        public static bool CheckDateAvailability(IEnumerable<Bokning> bokningar, Bokning bokning)
        {
            var bokningarDatum = new List<DateTime>();

            var bokningDatum = new List<DateTime>();

            var datum = new DateTime();

            foreach (var b in bokningar) // Alla redan uppbokade datum
            {
                datum = b.Startdatum;

                while (datum <= b.Slutdatum)
                {
                    bokningarDatum.Add(datum);
                    datum = datum.AddDays(1);
                }
            }

            datum = bokning.Startdatum;

            while (datum <= bokning.Slutdatum) // Alla datum för aktuell bokning
            {
                bokningDatum.Add(datum);
                datum = datum.AddDays(1);
            }

            foreach (var b in bokningDatum) // Jämför aktuella datum mot uppbokade
            {
                if (bokningarDatum.Contains(b))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValidateNewBokning(Bokning bokning)
        {
            if(bokning.Startdatum < DateTime.Now)
            {
                return false;
            }

            if(bokning.Slutdatum <= DateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}
