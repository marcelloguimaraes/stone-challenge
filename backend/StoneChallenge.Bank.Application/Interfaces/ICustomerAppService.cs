using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Application.Interfaces
{
    public interface ICustomerAppService : IDisposable
    {
        Task RegisterAsync(CustomerViewModel accountViewModel);
        Task<CustomerViewModel> GetByIdAsync(string id);
        Task<Customer> GetByCpf(string cpf);
    }
}
