using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoneChallenge.Bank.Domain.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public int AccountNumber { get; set; }
        public int Agency { get; set; }
        public double Balance { get; set; } = 0;
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public virtual List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual Customer Customer { get; set; }

        protected Account() { }

        public Account(string accountId, int agency, string customerId)
        {
            AccountId = accountId;
            AccountNumber = GenerateAccountNumber();
            Agency = agency;
            CustomerId = customerId;
        }

        public void Transfer(Account targetAccount, double value)
        {
            if(targetAccount == null)
            {
                throw new ArgumentNullException(nameof(targetAccount), "Conta destino é obrigatória");
            }

            ValidateNegativeValue(value);
            ValidateValueGreaterThanBalance(value);

            double fee = 1; // R$ 1,00 sobre o valor transferido
            double valueWithFee = value + fee;

            WithdrawValue(valueWithFee);
            
            targetAccount.DepositValue(value);

            AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Transfer, DateTime.Now, -valueWithFee, AccountId, $"para a conta {targetAccount.AccountNumber}"));
            targetAccount.AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Transfer, DateTime.Now, value, targetAccount.AccountId, $"recebida da conta {AccountNumber}"));
        }

        public void WithDraw(double value)
        {
            ValidateNegativeValue(value);
            ValidateValueGreaterThanBalance(value);

            double fee = 4;  // R$ 4,00 sobre o valor sacado

            double valueWithFee = value + fee;
            AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Withdraw, DateTime.Now, -valueWithFee, AccountId));

            WithdrawValue(valueWithFee);
        }

        public void WithdrawValue(double value)
        {
            Balance -= value;
        }


        public void Deposit(double value)
        {
            ValidateNegativeValue(value);
            double fee = value * 0.01; // 1% sobre o valor depositado
            double valueWithFee = value - fee;
            AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Deposit, DateTime.Now, valueWithFee, AccountId));
            DepositValue(valueWithFee);
        }

        public void DepositValue(double value)
        {
            Balance += value;
        }

        private void ValidateValueGreaterThanBalance(double value)
        {
            if (value > Balance)
            {
                throw new ArgumentException($"Valor com a taxa superior ao saldo atual");
            }
        }

        private void ValidateNegativeValue(double value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Valor inválido");
            }
        }

        private int GenerateAccountNumber()
        {
            return new Random().Next(1, 999999);
        }

        public virtual void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
        }

    }
}
