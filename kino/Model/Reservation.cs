using System;
using System.ComponentModel.DataAnnotations;

namespace kino.Database
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }
        public int? SetX { get; set; }
        public int? SetY { get; set; }
        public DateTime? Expiration { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public int ScreeningId { get; set; }
        public Screening Screening { get; set; }
    }
}
