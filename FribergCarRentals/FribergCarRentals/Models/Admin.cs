using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FribergCarRentals.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Admin : User
    {
        public Admin()
        {
            this.Role = "admin";
        }
    }
}
