using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace sakanat.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
      
        [Required]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "رقم قومي غير صحيح")]
        public string NationalId { get; set; }
    }
}
