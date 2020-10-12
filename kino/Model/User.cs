using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace kino.Database
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
