using System;

namespace StoneChallenge.Bank.Domain.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public string AccountId { get; set; }
        public virtual Account Account { get; set; }

        protected Transaction() { }

        public Transaction(string transactionId, TransactionType transactionType, DateTime date, double value, string accountId)
        {
            TransactionId = transactionId;
            TransactionType = transactionType;
            Date = date;
            AccountId = accountId;
            Value = value;
        }
    }

    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2,
        Transfer = 3
    }
}
