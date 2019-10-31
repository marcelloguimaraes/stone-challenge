using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class WithdrawViewModel : SourceAccountViewModel
    {
        [Required(ErrorMessage = "Valor de saque é obrigatório")]
        [Range(0.0, 1000000.0, ErrorMessage = "Valor de saque inválido")]
        public double Value { get; set; }
    }
}
