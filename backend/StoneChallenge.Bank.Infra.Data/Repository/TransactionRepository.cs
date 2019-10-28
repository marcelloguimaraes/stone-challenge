using Microsoft.EntityFrameworkCore;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Domain.Models;
using StoneChallenge.Bank.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Infra.Data.Repository
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(BankContext context)
            : base(context)
        {

        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId)
        {
            return await DbSet.Where(t => t.AccountId == accountId).ToListAsync();
        }
    }
}
