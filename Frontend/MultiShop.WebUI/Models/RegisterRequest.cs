using System.ComponentModel.DataAnnotations;

namespace MultiShop.WebUI.Models
{
    public class RegisterRequest
    {
        public string? Username { get; set; }

        [Required(ErrorMessage = "Ad zorunludur")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur")]
        public string Surname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þifre zorunludur")]
        [MinLength(6, ErrorMessage = "Þifre en az 6 karakter olmalýdýr")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Þifreler eþleþmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}