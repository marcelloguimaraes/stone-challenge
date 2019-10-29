using System;

namespace StoneChallenge.Bank.Application.ViewModels
{
    public class TransactionViewModel
    {
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public DateTime Date { get; set; }
        public string DateFormatted { get; set; }
        public double Value { get; set; }
        public string Note { get; set; }
    }
}
