using StoneChallenge.Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Domain.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId);
    }
}
