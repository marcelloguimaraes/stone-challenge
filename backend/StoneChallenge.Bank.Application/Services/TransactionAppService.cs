using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Application.Services
{
    public class TransactionAppService : ITransactionAppService
    {
        //private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionAppService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(string accountId)
        {
            return await _transactionRepository.GetByAccountIdAsync(accountId);
        }
    }
}
