using System.ComponentModel.DataAnnotations;

namespace MultiShop.WebUI.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Kullanýcý adý zorunludur")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þifre zorunludur")]
        public string Password { get; set; } = string.Empty;
    }
}