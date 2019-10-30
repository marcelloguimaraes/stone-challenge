using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StoneChallenge.Bank.API.Auth;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;
using static StoneChallenge.Bank.Application.ViewModels.AuthViewModel;

namespace StoneChallenge.Bank.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthSettings _authSettings;
        private readonly IMapper _mapper;
        private readonly IAccountAppService _accountAppService;
        private readonly ICustomerAppService _customerAppService;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AuthSettings> AuthSettings,
                              IMapper mapper,
                              IAccountAppService accountAppService,
                              ICustomerAppService customerAppService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _authSettings = AuthSettings.Value;
            _mapper = mapper;
            _accountAppService = accountAppService;
            _customerAppService = customerAppService;
        }

        [HttpPost("open-account")]
        public async Task<ActionResult> OpenAccount(OpenAccountViewModel openAccount)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

                var identityUser = new IdentityUser
                {
                    UserName = openAccount.Email,
                    Email = openAccount.Email,
                    EmailConfirmed = true
                };

                var customer = await _customerAppService.GetByCpf(openAccount.Customer.Cpf);

                if (customer != null)
                {
                    return Conflict("Já existe um cliente com o cpf informado");
                }

                var result = await _userManager.CreateAsync(identityUser, openAccount.Password);

                if (!result.Succeeded) return BadRequest(result.Errors.Select(i => i.Description));

                var account = await _accountAppService.RegisterAsync(openAccount);

                return Ok(new { account.Agency, account.AccountNumber, message = "Conta aberta com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao abrir conta {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginAccountViewModel loginAccount)
        {

            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors));

            var result = await _signInManager.PasswordSignInAsync(loginAccount.Email, loginAccount.Password, false, true);

            if (!result.Succeeded)
            {
                return BadRequest("Usuário ou senha inválidos");
            }

            var user = await _userManager.FindByEmailAsync(loginAccount.Email);

            var token = await GenerateJwt(loginAccount.Email);

            var userObj = new
            {
                token,
                user = new
                {
                    user.UserName,
                    user.Email,
                    user.Id
                }
            };

            return Ok(userObj);
        }

        private async Task<string> GenerateJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new[]
                //{
                //    new Claim(ClaimTypes.Name, user.Id)
                //}),
                Issuer = _authSettings.Issuer,
                Audience = _authSettings.Audience,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
    }
}
