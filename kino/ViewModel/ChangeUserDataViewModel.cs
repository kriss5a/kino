using System.ComponentModel.DataAnnotations;

namespace kino.Controllers
{
    public class ChangeUserDataViewModel
    {
        [Required]
        public string FullName { get; set; }
    }
}