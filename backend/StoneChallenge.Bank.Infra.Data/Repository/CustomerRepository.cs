using Microsoft.EntityFrameworkCore;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Domain.Models;
using StoneChallenge.Bank.Infra.Data.Context;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Infra.Data.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(BankContext context)
            : base(context)
        {

        }

        public async Task<Customer> GetByCpf(string cpf)
        {
            return await DbSet.Include(c => c.Account)
                                .ThenInclude(a => a.Transactions)
                              .FirstOrDefaultAsync(c => c.Cpf == cpf);
        }
    }
}
