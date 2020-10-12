using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace kino.Database
{
    public class Screening
    {
        [Key]
        public int ScreeningId { get; set; }
        public DateTime DateTime { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int ScreeningRoomId { get; set; }
        public ScreeningRoom ScreeningRoom { get; set; }
        
        //public virtual ICollection<Reservation> Reservations { get; set; }

        public void Patch(Screening screening)
        {
            DateTime = screening.DateTime;
            MovieId = screening.MovieId;
            ScreeningId = screening.ScreeningId;
        }
    }
}
