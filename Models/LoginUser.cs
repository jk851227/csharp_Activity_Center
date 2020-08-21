using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    [NotMapped]
    public class LoginUser
    {
        [Required(ErrorMessage="is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string LoginEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 characters or more")]
        [Display(Name = "Password")]
        public string LoginPassword { get; set; }
    }
}