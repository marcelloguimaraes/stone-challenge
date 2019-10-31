using Moq;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using Xunit;

namespace StoneChallenge.Bank.Tests.Models
{
    public class AccountTests
    {

        //[Fact]
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Withdraw_Given_InvalidValue_Then_ThrowArgumentException(double value)
        {
            var account = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 20
            };

            var resultMessage = Assert.Throws<ArgumentException>(() => account.WithDraw(value));
            var expectedMessage = "Valor inválido";
            Assert.Equal(expectedMessage, resultMessage.Message);
        }

        [Fact]
        public void Withdraw_Given_Value_GreaterThanBalance_Then_ThrowArgumentException()
        {
            var account = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 0
            };

            var fee = 4;

            var resultMessage = Assert.Throws<ArgumentException>(() => account.WithDraw(20));
            var expectedMessage = $"Valor com a taxa superior ao saldo atual. Taxa: R$ {fee}";
            Assert.Equal(expectedMessage, resultMessage.Message);
        }

        [Fact]
        public void Withdraw_Given_WithdrawWithFee_Then_ReturnCorrectBalance()
        {
            var account = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 100
            };

            var value = 10;
            var balanceExpected = 86; // 100 - (10 + 4(taxa))

            account.WithDraw(value);

            Assert.Equal(balanceExpected, account.Balance);
        }

        [Fact]
        public void Withdraw_Given_ValidWithdraw_Then_MustAddOneTransaction()
        {
            var value = 10;
            var mock = new Mock<Account>();

            mock.Setup(a => a.AddTransaction(It.IsAny<Transaction>()));

            mock.Object.Balance = 100;

            mock.Object.WithDraw(value);

            mock.Verify(a => a.AddTransaction(It.IsAny<Transaction>()), Times.Exactly(1));
        }

        [Fact]
        public void Deposit_Given_ValidDeposit_Then_MustAddOneTransaction()
        {
            var value = 10;
            var mock = new Mock<Account>();

            mock.Setup(a => a.AddTransaction(It.IsAny<Transaction>()));

            mock.Object.Balance = 100;

            mock.Object.Deposit(value);

            mock.Verify(a => a.AddTransaction(It.IsAny<Transaction>()), Times.Exactly(1));
        }

        [Fact]
        public void Transfer_Given_ValidTransfer_Then_MustAddTwoTransactions()
        {
            var value = 10;
            var mockSourceAccount = new Mock<Account>();
            var mockTargetAccount = new Mock<Account>();

            mockSourceAccount.Setup(a => a.AddTransaction(It.IsAny<Transaction>()));
            mockTargetAccount.Setup(a => a.AddTransaction(It.IsAny<Transaction>()));

            var targetAccount = new Account("47a77e3a-4db7-44bc-aa01-bcab6b58e191",
                                            3032,
                                            "b6f5a05b-a466-4bcc-977a-507e9745a227");

            mockSourceAccount.Object.Balance = 100;
            mockTargetAccount.Object.Balance = 100;

            mockSourceAccount.Object.Transfer(mockTargetAccount.Object, value);

            mockSourceAccount.Verify(a => a.AddTransaction(It.IsAny<Transaction>()), Times.Exactly(1));
            mockTargetAccount.Verify(a => a.AddTransaction(It.IsAny<Transaction>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Deposit_Given_InvalidValue_Then_ThrowArgumentException(double value)
        {
            var account = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 20
            };

            var resultMessage = Assert.Throws<ArgumentException>(() => account.Deposit(value));
            var expectedMessage = "Valor inválido";
            Assert.Equal(expectedMessage, resultMessage.Message);
        }

        [Fact]
        public void Deposit_Given_DepositWithFee_Then_ReturnCorrectBalance()
        {
            var account = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 100
            };

            var value = 10;
            var balanceExpected = 109.9; // 100 + (10 + 0.01(taxa))

            account.Deposit(value);

            Assert.Equal(balanceExpected, account.Balance);
        }


        [Fact]
        public void Transfer_Given_NullTargetAccount_Then_ThrowArgumentException()
        {
            var account = new Account("47a77e3a-4db7-44bc-aa01-bcab6b58e191	",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227");

            double value = 20;

            var resultMessage = Assert.Throws<ArgumentException>(() => account.Transfer(null, value));
            var expectedMessage = "Conta destino é obrigatória";
            Assert.Equal(expectedMessage, resultMessage.Message);
        }

        [Fact]
        public void Transfer_Given_TransferWithFee_Then_ReturnCorrectBalance()
        {
            var sourceAccount = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 100
            };

            var targetAccount = new Account("d3067022-5fab-4a42-9c80-6dab8b231a21",
                                      3031,
                                      "b84385d7-6b8c-459e-bf29-db46857fd40c")
            {
                Balance = 0
            };

            var value = 10;
            var balanceSourceAccountExpected = 89; // 100 - (10 + 1(taxa))
            var balanceTargetAccountExpected = 10;

            sourceAccount.Transfer(targetAccount, value);

            Assert.Equal(balanceSourceAccountExpected, sourceAccount.Balance);
            Assert.Equal(balanceTargetAccountExpected, targetAccount.Balance);
        }

        [Fact]
        public void Transfer_Given_Transfer_Then_DepositTransactionMustBeInsertedWithoutFee()
        {
            var sourceAccount = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 100
            };

            var targetAccount = new Account("d3067022-5fab-4a42-9c80-6dab8b231a21",
                                      3031,
                                      "b84385d7-6b8c-459e-bf29-db46857fd40c")
            {
                Balance = 0
            };

            double value = 10;
            double valueTransactionExpected = 10;

            sourceAccount.Transfer(targetAccount, value);

            var transaction = targetAccount.Transactions.LastOrDefault();
            Assert.Equal(valueTransactionExpected, transaction.Value);
        }
        [Fact]
        public void Transfer_Given_Transfer_Then_WithdrawTransactionMustBeInsertedWithFeeAndAsANegativeValue()
        {
            var sourceAccount = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227")
            {
                Balance = 100
            };

            var targetAccount = new Account("d3067022-5fab-4a42-9c80-6dab8b231a21",
                                      3031,
                                      "b84385d7-6b8c-459e-bf29-db46857fd40c")
            {
                Balance = 0
            };

            double value = 10;
            double valueTransactionExpected = -11;

            sourceAccount.Transfer(targetAccount, value);

            var transaction = sourceAccount.Transactions.LastOrDefault();
            Assert.Equal(valueTransactionExpected, transaction.Value);
        }
    }
}
