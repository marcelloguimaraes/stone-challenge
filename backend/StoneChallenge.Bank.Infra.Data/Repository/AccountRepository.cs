using Microsoft.EntityFrameworkCore;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Domain.Models;
using StoneChallenge.Bank.Infra.Data.Context;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Infra.Data.Repository
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(BankContext context)
            : base(context)
        {

        }

        public async Task<Account> GetByAccountNumberAndAgencyAsync(int accountNumber, int agency)
        {
            return await DbSet.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber && a.Agency == agency);
        }

        public async Task<Account> GetByAccountNumberAsync(int accountNumber)
        {
            return await DbSet.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<Account> GetByUserId(string userId)
        {
            return await DbSet.Include(a => a.Customer)
                              .FirstOrDefaultAsync(a => a.UserId == userId);
        }
    }
}
