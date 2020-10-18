using System;
using System.ComponentModel.DataAnnotations;

namespace kino.Database
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }
        public int? SeatX { get; set; }
        public int? SeatY { get; set; }
        public DateTime? Expiration { get; set; }
        public int Priority { get; set; }
        public bool IsConfirmed { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        public int ScreeningId { get; set; }
        public Screening Screening { get; set; }
    }
}
