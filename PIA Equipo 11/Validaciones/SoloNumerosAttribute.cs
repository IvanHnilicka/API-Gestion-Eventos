using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace PIA_Equipo_11.Validaciones
{
    public class SoloNumerosAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string cadena = value.ToString();

            if (cadena == null || cadena.IsNullOrEmpty())
            {
                return ValidationResult.Success;
            }

            foreach (char c in cadena)
            {
                if (char.IsLetter(c))
                {
                    return new ValidationResult("Error. Campo Telefono debe contener solo numeros");
                }
            }

            return ValidationResult.Success;
        }
    }
}