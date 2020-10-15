using System.ComponentModel.DataAnnotations;

namespace kino.ViewModel
{
    public class ReservationViewModel
    {
        [Required]
        public int ScreeningId { get; set; }
        [Required]
        public int SeatX { get; set; }
        [Required]
        public int SeatY { get; set; }
    }
}
