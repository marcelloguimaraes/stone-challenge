using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class WithdrawViewModel : SourceAccountViewModel
    {
        [Required(ErrorMessage = "Valor de saque é obrigatório")]
        [Range(0.0, 1000000.0, ErrorMessage = "Valor de saque deve ser entre 0 e 1000000.0")]
        public double Value { get; set; }
    }
}
