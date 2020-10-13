using System.ComponentModel.DataAnnotations;

namespace KnmBackend.Controllers
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}