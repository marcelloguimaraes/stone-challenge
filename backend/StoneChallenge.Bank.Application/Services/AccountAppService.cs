using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Interfaces;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Threading.Tasks;
using static StoneChallenge.Bank.Application.ViewModels.AuthViewModel;

namespace StoneChallenge.Bank.Application.Services
{
    public class AccountAppService : IAccountAppService
    {
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<IdentityUser> _userManager;


        public AccountAppService(IMapper mapper,
                                 IAccountRepository accountRepository,
                                 UserManager<IdentityUser> userManager)
        {
            _mapper = mapper;
            _accountRepository = accountRepository;
            _userManager = userManager;
        }

        public  Task<Account> GetByAccountNumberAndAgencyAsync(int accountNumber, int agency)
        {
            return _accountRepository.GetByAccountNumberAndAgencyAsync(accountNumber, agency);
        }

        public async Task<Account> RegisterAsync(OpenAccountViewModel openAccount)
        {
            // cria cliente na memória
            var customer = _mapper.Map<Customer>(openAccount.Customer);
            customer.CustomerId = Guid.NewGuid().ToString();

            // busca usuário criado para vincular a conta
            var user = await _userManager.FindByEmailAsync(openAccount.Email);

            // cria conta
            var account = new Account(Guid.NewGuid().ToString(), 3032, customer.CustomerId);
            account.UserId = user.Id;
            account.Customer = customer;

            // salva a conta e o cliente juntos com o ef core
            await _accountRepository.AddAsync(account);

            return account;
        }

        public async Task TransferAsync(Account sourceAccount, Account targetAccount, double value)
        {
            sourceAccount.Transfer(targetAccount, value);

            await _accountRepository.UpdateAsync(sourceAccount);
            await _accountRepository.UpdateAsync(targetAccount);
        }

        public async Task WithDraw(Account account, double value)
        {
            account.WithDraw(value);
            await _accountRepository.UpdateAsync(account);
        }

        public async Task Deposit(Account account, double value)
        {
            account.Deposit(value);
            await _accountRepository.UpdateAsync(account);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<Account> GetByAccountNumberAsync(int accountNumber)
        {
            return await _accountRepository.GetByAccountNumberAsync(accountNumber);
        }

        public async Task<Account> GetByUserId(string userId)
        {
            return await _accountRepository.GetByUserId(userId);
        }
    }
}
