using System;
using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class CustomerViewModel
    {
        [Required(ErrorMessage = "Cpf é obrigatório")]
        [MaxLength(11, ErrorMessage = "Cpf deve conter no máximo 11 caracteres")]
        public string Cpf { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(255, ErrorMessage = "Nome deve conter no máximo 255 caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Data de nascimento é obrigatório")]
        public DateTime BirthDate { get; set; }
    }
}
