using System.ComponentModel.DataAnnotations;

namespace kino.Database
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        [MaxLength(100), Required, ]
        public string Title { get; set; }
        [MaxLength(100), Required]
        public string Genres { get; set; }
        public uint Time{ get; set; }

        public void Patch(Movie movie)
        {
            Title = movie.Title;
            Genres = movie.Genres;
            Time = movie.Time;
        }

        //public virtual ICollection<Screening> Screenings { get; set; }
    }
}
