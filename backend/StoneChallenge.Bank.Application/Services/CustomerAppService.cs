using AutoMapper;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Threading.Tasks;

namespace StoneChallenge.Bank.Application.Services
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerAppService(ICustomerRepository customerRepository,
                                  IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public Task<Customer> GetByCpf(string cpf)
        {
            return _customerRepository.GetByCpf(cpf);
        }

        public async Task<CustomerViewModel> GetByIdAsync(string id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return null;
            return _mapper.Map<CustomerViewModel>(customer);
        }

        public async Task RegisterAsync(CustomerViewModel accountViewModel)
        {
            var customer = _mapper.Map<Customer>(accountViewModel);
            customer.CustomerId = Guid.NewGuid().ToString();
            await _customerRepository.AddAsync(customer);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
