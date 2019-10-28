using StoneChallenge.Bank.Domain.Models;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetByCpf(string cpf);
    }
}