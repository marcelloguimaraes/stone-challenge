using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.ViewModels;

namespace StoneChallenge.Bank.API.Controllers
{
    [Route("api/customers")]
    [Authorize]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerAppService _customerAppService;
        private readonly IAccountAppService _accountAppService;
        private readonly IMapper _mapper;


        public CustomerController(ICustomerAppService customerAppService = null,
                                  IAccountAppService accountAppService = null,
                                  IMapper mapper = null)
        {
            _customerAppService = customerAppService;
            _accountAppService = accountAppService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("transactions/{cpf}")]
        public async Task<ActionResult<IEnumerable<TransactionViewModel>>> GetTransactionsByCpf(string cpf)
        {
            try
            {
                if (string.IsNullOrEmpty(cpf))
                {
                    return BadRequest("Cpf precisa ser informado");
                }

                var customer = await _customerAppService.GetByCpf(cpf);

                if (customer == null)
                {
                    return NotFound("Cliente não encontrado");
                }

                var transactions = _mapper.Map<IEnumerable<TransactionViewModel>>(customer.Account.Transactions);

                if (transactions.Count() == 0 || transactions == null)
                {
                    return NotFound("Nenhuma transação encontrada para o cliente");
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}