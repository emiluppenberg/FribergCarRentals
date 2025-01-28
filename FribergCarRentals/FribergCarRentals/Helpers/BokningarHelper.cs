using FribergCarRentals.Models;

namespace FribergCarRentals.Helpers
{
    public static class BokningarHelper
    {
        public static bool ValidateBokning(Bokning bokning)
        {
            if (bokning.Startdatum > bokning.Slutdatum)
            {
                return false;
            }

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
