using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace kino.Database
{
    public class ScreeningRoom
    {
        [Key]
        public int ScreeningRoomId { get; set; }
        [Required]
        public string Name { get; set; }
        public uint Height { get; set; }
        public uint Width { get; set; }

        //public virtual ICollection<Screening> Screenings { get; set; }

        public void Patch(ScreeningRoom room)
        {
            Name = room.Name;
            Height = room.Height;
            Width = room.Width;
        }
    }
}
