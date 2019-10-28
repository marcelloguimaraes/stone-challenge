using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class TransferViewModel
    {
        public SourceAccountViewModel SourceAccount { get; set; }
        public TargetAccountViewModel TargetAccount { get; set; }
        [Required(ErrorMessage="Valor de transferência é obrigatório")]
        [Range(1, 1000000, ErrorMessage = "Valor de transferência inválido")]
        public double Value { get; set; }
    }

    public class SourceAccountViewModel
    {
        [Required(ErrorMessage = "Número da conta é obrigatório")]
        public int AccountNumber { get; set; }
        [Required(ErrorMessage = "Número da agência é obrigatório")]
        public int Agency { get; set; }
    }

    public class TargetAccountViewModel : SourceAccountViewModel
    {

    }
}
