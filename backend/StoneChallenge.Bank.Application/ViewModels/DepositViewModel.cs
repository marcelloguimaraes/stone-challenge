using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class DepositViewModel : SourceAccountViewModel
    {
        [Required(ErrorMessage = "Valor de depósito é obrigatório")]
        [Range(0.01, 1000000.0, ErrorMessage = "Valor de depósito inválido deve ser entre 0.01 e 1000000")]
        public double Value { get; set; }
    }
}
