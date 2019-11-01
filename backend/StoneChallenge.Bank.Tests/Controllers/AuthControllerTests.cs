using Microsoft.AspNetCore.Mvc;
using StoneChallenge.Bank.API.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Options;
using StoneChallenge.Bank.API.Auth;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Domain.Models;
using static StoneChallenge.Bank.Application.ViewModels.AuthViewModel;
using StoneChallenge.Bank.Application.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading;

namespace StoneChallenge.Bank.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task OpenAccount_Given_InvalidModelState_Then_ReturnBadRequestObjectResult()
        {
            var authSettings = new Mock<IOptions<AuthSettings>>();
            
            // Arrange
            var controller = new AuthController(AuthSettings: authSettings.Object);
            controller.ModelState.AddModelError("OpenAccountViewModel", "Informe um modelo válido");
            //Act
            var result = await controller.OpenAccount(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task OpenAccount_Given_CustomerAlreadyHasCpfRegistered_Then_ReturnConflictObjectResult()
        {
            var mockCustomerService = new Mock<ICustomerAppService>();
            var mockAuthSettings = new Mock<IOptions<AuthSettings>>();

            var customer = new Customer("jak1i2k3i-k12i3k-12ki3k1", "1645545852", "Teste", DateTime.Now);
            var openAccountViewModel = new OpenAccountViewModel()
            {
                Agency = 3032,
                Customer = new CustomerViewModel() { BirthDate = customer.BirthDate, Cpf = customer.Cpf, Name = customer.Name },
                Email = "marcello@hotmail.com",
                Password = "Marcello@123"
            };

            mockCustomerService.Setup(c => c.GetByCpf(It.IsAny<string>())).ReturnsAsync(customer);

            // Arrange
            var controller = new AuthController(customerAppService: mockCustomerService.Object, AuthSettings: mockAuthSettings.Object);

            //Act
            var result = await controller.OpenAccount(openAccountViewModel);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        //[Fact]
        //public async Task OpenAccount_Given_CreateAsyncUserManagerResultNotSucceeded_Then_ReturnIdentityResultErrors()
        //{
        //    var mockCustomerService = new Mock<ICustomerAppService>();
        //    var mockAuthSettings = new Mock<IOptions<AuthSettings>>();

        //    var store = new Mock<IUserStore<IdentityUser>>();

        //    var identityUser = new IdentityUser()
        //    {
        //        Email = "testedda.com", UserName = "wrong"
        //    };

        //    var userManager = new UserManager<IdentityUser>(store.Object, null, null, null, null, null, null, null, null);

        //    store.Setup(s => s.CreateAsync(identityUser, new CancellationToken() { }));

        //    var customer = new Customer("jak1i2k3i-k12i3k-12ki3k1", "1645545852", "Teste", DateTime.Now);
        //    var openAccountViewModel = new OpenAccountViewModel()
        //    {
        //        Agency = 3032,
        //        Customer = new CustomerViewModel() { BirthDate = customer.BirthDate, Cpf = customer.Cpf, Name = customer.Name },
        //        Email = "marcello@hotmail.com",
        //        Password = "12j3123j"
        //    };

        //    mockCustomerService.Setup(c => c.GetByCpf(It.IsAny<string>())).ReturnsAsync((Customer)null);


        //    //mockUserManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), openAccountViewModel.Password)).ReturnsAsync()

        //    // Arrange
        //    var controller = new AuthController(customerAppService: mockCustomerService.Object, AuthSettings: mockAuthSettings.Object, userManager: userManager);
        //    //Act
        //    var result = await controller.OpenAccount(openAccountViewModel);

        //    // Assert
        //    Assert.IsType<IdentityResult>(result);
        //}
    }
}
