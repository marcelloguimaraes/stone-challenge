using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;

namespace StoneChallenge.Bank.API.Controllers
{
    [Route("api/accounts")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountAppService _accountAppService;
        private readonly ITransactionAppService _transactionAppService;
        private readonly ICustomerAppService _customerAppService;
        private readonly IMapper _mapper;


        public AccountController(IAccountAppService accountAppService = null,
                                 ITransactionAppService transactionAppService = null,
                                 ICustomerAppService customerAppService = null,
                                 IMapper mapper = null)
        {
            _accountAppService = accountAppService;
            _transactionAppService = transactionAppService;
            _customerAppService = customerAppService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("transactions")]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactionsByAccountNumberAndAgency([FromBody] SourceAccountViewModel sourceAccountViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("TransactionViewModelInvalid", "Informe um modelo válido");
                return BadRequest(ModelState);
            }

            var account = await _accountAppService.GetByAccountNumberAndAgencyAsync(sourceAccountViewModel.AccountNumber, sourceAccountViewModel.Agency);

            if (account == null)
            {
                return NotFound("Conta não encontrada");
            }

            var transactions = await _transactionAppService.GetByAccountIdAsync(account.AccountId);

            if (!transactions.Any())
            {
                return NotFound("Nenhuma transação encontrada para esta conta");
            }

            var result = _mapper.Map<IEnumerable<TransactionViewModel>>(transactions.OrderByDescending(t => t.Date));

            return Ok(result);
        }

        [HttpPost]
        [Route("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferViewModel transferViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("TransferViewModelInvalid", "Informe um modelo válido");
                    return BadRequest(ModelState);
                }

                if(transferViewModel.SourceAccount.AccountNumber == transferViewModel.TargetAccount.AccountNumber)
                {
                    return BadRequest("Não é permitido transferir para mesma conta");
                }

                var (sourceAccountModel, targetAccountModel) = (transferViewModel.SourceAccount, transferViewModel.TargetAccount);

                var sourceAccount = await _accountAppService.GetByAccountNumberAndAgencyAsync(sourceAccountModel.AccountNumber, sourceAccountModel.Agency);

                if (sourceAccount == null)
                {
                    return NotFound("Conta de origem não encontrada");
                }

                var targetAccount = await _accountAppService.GetByAccountNumberAndAgencyAsync(targetAccountModel.AccountNumber, targetAccountModel.Agency);

                if (targetAccount == null)
                {
                    return NotFound("Conta de destino não encontrada");
                }

                await _accountAppService.TransferAsync(sourceAccount, targetAccount, transferViewModel.Value);

                var sourceAccountMapped = _mapper.Map<Account, AccountListViewModel>(sourceAccount);
                var targetAccountMapped = _mapper.Map<Account, AccountListViewModel>(targetAccount);

                var result = new TransferListViewModel()
                {
                    SourceAccount = sourceAccountMapped,
                    TargetAccount = targetAccountMapped
                };

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao realizar transferência. {ex.Message}");
            }
        }

        [HttpPost]
        [Route("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawViewModel withdrawViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("WithdrawViewModelInvalid", "Informe um modelo válido");
                    return BadRequest(ModelState);
                }

                var account = await _accountAppService.GetByAccountNumberAndAgencyAsync(withdrawViewModel.AccountNumber, withdrawViewModel.Agency);

                if (account == null)
                {
                    return NotFound("Conta não encontrada");
                }

                await _accountAppService.WithDraw(account, withdrawViewModel.Value);

                var result = _mapper.Map<AccountListViewModel>(account);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao realizar saque. {ex.Message}");
            }
        }

        [HttpPost]
        [Route("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositViewModel depositViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("DepositViewModelInvalid", "Informe um modelo válido");
                    return BadRequest(ModelState);
                }

                var account = await _accountAppService.GetByAccountNumberAndAgencyAsync(depositViewModel.AccountNumber, depositViewModel.Agency);

                if (account == null)
                {
                    return NotFound("Conta não encontrada");
                }

                await _accountAppService.Deposit(account, depositViewModel.Value);

                var result = _mapper.Map<AccountListViewModel>(account);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao realizar depósito. {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AccountViewModel accountViewModel)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    ModelState.AddModelError("AccountViewModelInvalid", "Informe um modelo válido");
                //    return BadRequest(ModelState);
                //}

                //var customer = await _customerAppService.GetByIdAsync(accountViewModel.CustomerId);

                //if (customer == null)
                //{
                //    return NotFound("Cliente não encontrado");
                //}

                //var account = await _accountAppService.GetByAccountNumberAsync(accountViewModel.AccountNumber);

                //if (account != null)
                //{
                //    return Conflict(new { message = "Já existe uma conta para o número de conta informado" });
                //}

                //account = _mapper.Map<Account>(accountViewModel);

                //await _accountAppService.RegisterAsync(account);
                return Ok("Conta criada com sucesso");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao realizar depósito. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("users/{userId}")]
        public async Task<ActionResult<Account>> GetByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Id do usuário não informado");
                }

                var account = await _accountAppService.GetByUserId(userId);

                if (account == null)
                {
                    return NotFound("Conta não encontrada");
                }

                var result = _mapper.Map<AccountListViewModel>(account);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao recuperar conta do usuário. " + ex.Message);
            }
        }
    }
}
