using System.ComponentModel.DataAnnotations;

namespace sakanat.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "رقم قومي غير صحيح")]
        public string NationalId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور غير متطابقة")]
        public string ConfirmPassword { get; set; }
    }
}
