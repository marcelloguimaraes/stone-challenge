namespace StoneChallenge.Bank.Application.ViewModels
{
    public class AccountListViewModel
    {
        public int AccountNumber { get; set; }
        public int Agency { get; set; }
        public double Balance { get; set; } = 0;
        public string CustomerName { get; set; }
    }
}
