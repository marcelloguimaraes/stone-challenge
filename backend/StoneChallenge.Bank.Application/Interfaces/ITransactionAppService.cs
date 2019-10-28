using StoneChallenge.Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Application.Interfaces
{
    public interface ITransactionAppService : IDisposable
    {
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId);
    }
}
