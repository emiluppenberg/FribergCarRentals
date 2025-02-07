using FribergCarRentals.Models;

namespace FribergCarRentals.Helpers
{
    public static class BokningarHelper
    {
        public static string? ValidateExistingBokning(Bokning bokning, Bokning model)
        {
            if (bokning.Startdatum <= DateTime.Today)
            {
                if (bokning.Startdatum != model.Startdatum)
                {
                    return "Startdatum kan inte ändras för en pågående bokning";
                }
            }

            if (model.Startdatum <= DateTime.Today && model.Startdatum != bokning.Startdatum)
            {
                return "Startdatum måste vara minst en dag framåt";
            }

            if (model.Startdatum >= model.Slutdatum)
            {
                return "Startdatum kan inte vara längre fram eller samma som slutdatum";
            }

            if (model.Slutdatum <= DateTime.Today)
            {
                return "Slutdatum måste vara framåt i tiden";
            }

            return null;
        }

        public static string? ValidateNewBokning(Bokning model)
        {
            if (model.Startdatum <= DateTime.Today)
            {
                return "Startdatum måste vara minst en dag framåt";
            }

            if (model.Startdatum >= model.Slutdatum)
            {
                return "Startdatum kan inte vara längre fram eller samma som slutdatum";
            }

            if (model.Slutdatum <= DateTime.Today)
            {
                return "Slutdatum måste vara framåt i tiden";
            }

            return null;
        }

        public static string? ValidateAvslutaBokning(Bokning bokning, Bokning model)
        {
            if (bokning.Startdatum > DateTime.Today)
            {
                return "Bokningen kan inte avslutas, startdatumet ligger framåt i tiden";
            }

            if (model.Slutdatum > DateTime.Today)
            {
                return "Bokningen kan inte avslutas, slutdatumet ligger framåt i tiden";
            }

            if (model.Slutdatum <= bokning.Startdatum)
            {
                return "Bokningen kan inte avslutas, slutdatum kan inte ligga före startdatum";
            }

            return null;
        }
    }
}
