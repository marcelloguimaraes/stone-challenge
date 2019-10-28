using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class DepositViewModel : SourceAccountViewModel
    {
        [Required(ErrorMessage = "Valor de depósito é obrigatório")]
        [Range(1, 1000000, ErrorMessage = "Valor de depósito inválido")]
        public double Value { get; set; }
    }
}
