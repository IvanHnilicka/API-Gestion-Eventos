using System.ComponentModel.DataAnnotations;

namespace PIA_Equipo_11.DTO
{
    public class CredencialesLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
