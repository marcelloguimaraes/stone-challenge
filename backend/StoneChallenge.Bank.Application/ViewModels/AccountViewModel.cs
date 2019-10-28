using System.ComponentModel.DataAnnotations;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class AccountViewModel : SourceAccountViewModel
    {
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public string CustomerId { get; set; }
    }
}
