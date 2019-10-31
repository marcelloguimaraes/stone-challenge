using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class DepositViewModel : SourceAccountViewModel
    {
        [Required(ErrorMessage = "Valor de depósito é obrigatório")]
        [Range(0.0, 1000000.0, ErrorMessage = "Valor de depósito inválido")]
        public double Value { get; set; }
    }
}
