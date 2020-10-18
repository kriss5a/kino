using kino.Database;

namespace kino.Controllers
{
    public class StartReservationResponse
    {
        public string Status { get; set; }
        public Reservation Reservation { get; set; }
    }
}
