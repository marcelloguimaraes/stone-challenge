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

        [NotMapped]
        private double _fee;
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
                throw new ArgumentException("Conta destino é obrigatória");
            }

            ApplyFee(TransactionType.Transfer);

            var valueWithFee = ValueWithFee(value);

            ValidateValueGreaterThanBalance(valueWithFee);

            WithDraw(valueWithFee, isTransfering: true);
            
            // Quando o depósito é chamado pela transferência deve enviar o valor sem a taxa, pois só quem paga a taxa é a conta origem
            targetAccount.Deposit(value, isTransfering: true);

            AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Transfer, DateTime.Now, valueWithFee * -1, AccountId, $"para a conta {targetAccount.AccountNumber}"));
            targetAccount.AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Transfer, DateTime.Now, value, targetAccount.AccountId, $"recebida da conta {AccountNumber}"));
        }

        public void WithDraw(double value, bool isTransfering = false)
        {
            ValidateNegativeValue(value);

            if (!isTransfering)
            {
                ApplyFee(TransactionType.Withdraw);
                value = ValueWithFee(value);
                ValidateValueGreaterThanBalance(value);
                AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Withdraw, DateTime.Now, value * -1, AccountId));
            }


            Balance -= value;
        }

        private void ValidateValueGreaterThanBalance(double value)
        {
            if (value > Balance)
            {
                throw new ArgumentException($"Valor com a taxa superior ao saldo atual. Taxa: R$ {_fee}");
            }
        }

        public void Deposit(double value, bool isTransfering = false)
        {
            ValidateNegativeValue(value);

            double valueWithFee = 0;

            if(!isTransfering)
            {
                ApplyFee(TransactionType.Deposit);
                valueWithFee = value - (value * _fee);
                AddTransaction(new Transaction(Guid.NewGuid().ToString(), TransactionType.Deposit, DateTime.Now, valueWithFee, AccountId));
            }

            // Se o depósito não estiver sendo chamado pela transferência(depósito comum)
            // deve depositar com taxa, se não deposita sem a taxa
            Balance += !isTransfering ? valueWithFee : value;
        }

        private void ApplyFee(TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Deposit:
                    _fee = 0.01; // 1% sobre o valor depositado
                    break;
                case TransactionType.Withdraw:
                    _fee = 4; // R$ 4,00 sobre o valor sacado
                    break;
                case TransactionType.Transfer:
                    _fee = 1; // R$ 1,00 sobre o valor transferido
                    break;
                default:
                    _fee = 0;
                    break;
            }
        }

        private double ValueWithFee(double value)
        {
            return value + _fee;
        }

        private void ValidateNegativeValue(double value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Valor inválido");
            }
        }

        public virtual void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
        }

        private int GenerateAccountNumber()
        {
            return new Random().Next(1, 999999);
        }
    }
}
