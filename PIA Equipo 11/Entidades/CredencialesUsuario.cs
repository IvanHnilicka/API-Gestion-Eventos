﻿using PIA_Equipo_11.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PIA_Equipo_11.Entidades
{
    public class CredencialesUsuario
    {
        [SoloLetras]
        public string Nombre { get; set; }
        [SoloNumeros]
        [MaxLength(10, ErrorMessage = "Campo Telefono debe contener 10 digitos")]
        [MinLength(10, ErrorMessage = "Campo Telefono debe contener 10 digitos")]
        public string Telefono { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
