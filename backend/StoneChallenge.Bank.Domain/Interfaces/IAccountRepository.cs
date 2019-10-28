using StoneChallenge.Bank.Domain.Models;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Domain.Interfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByAccountNumberAndAgencyAsync(int accountNumber, int agency);
        Task<Account> GetByAccountNumberAsync(int accountNumber);
        Task<Account> GetByUserId(string userId);
    }
}
