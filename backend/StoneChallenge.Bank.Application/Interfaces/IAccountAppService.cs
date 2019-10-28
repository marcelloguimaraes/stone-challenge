using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Threading.Tasks;
using static StoneChallenge.Bank.Application.ViewModels.AuthViewModel;

namespace StoneChallenge.Bank.Application.Interfaces
{
    public interface IAccountAppService : IDisposable
    {
        Task<Account> RegisterAsync(OpenAccountViewModel openAccount);

        Task<Account> GetByAccountNumberAndAgencyAsync(int accountNumber, int agency);
        Task<Account> GetByUserId(string userId);

        Task TransferAsync(Account sourceAccount, Account targetAccount, double value);
        Task WithDraw(Account account, double value);
        Task Deposit(Account account, double value);
        Task<Account> GetByAccountNumberAsync(int accountNumber);
    }
}
