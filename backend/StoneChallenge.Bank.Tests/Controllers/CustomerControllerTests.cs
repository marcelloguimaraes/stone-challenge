using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StoneChallenge.Bank.API.Controllers;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StoneChallenge.Bank.Tests.Controllers
{
    public class CustomerControllerTests
    {
        [Fact]
        public async Task GetTransactionsByCpf_GivenCpfIsNullOrEmpty_Then_ReturnBadRequestResult()
        {
            var controller = new CustomerController();

            var result = await controller.GetTransactionsByCpf(null);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.IsType<BadRequestObjectResult>(badRequestResult);
        }

        [Fact]
        public async Task GetTransactionsByCpf_Given_InvalidCpf_Then_ReturnNotFoundResult()
        {
            // Arrange
            var mockService = new Mock<ICustomerAppService>();

            mockService.Setup(service => service.GetByCpf(It.IsAny<string>())).ReturnsAsync((Customer)null);

            var controller = new CustomerController(customerAppService: mockService.Object);

            // Act
            var result = await controller.GetTransactionsByCpf("5445");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);


            // Assert
            Assert.IsType<NotFoundObjectResult>(notFoundResult);
        }
        [Fact]
        public async Task GetTransactionsByCpf_Given_NoneTransactionsFoundForCustomer_Then_ReturnNotFoundResult()
        {
            // Arrange
            var mockService = new Mock<ICustomerAppService>();
            var mockMapper = new Mock<IMapper>();

            var customer = new Customer(Guid.NewGuid().ToString(), "56544854698", "João", DateTime.Now);
            customer.Account = new Account("7127hs71hs17hs7217s2", 1232, "18187277163263");
            customer.Account.Transactions = new List<Transaction>()
            {
                new Transaction("h1y2hy31h2", TransactionType.Deposit, DateTime.Now, 12, "1231jjusju12j")
            };

            mockService.Setup(service => service.GetByCpf(It.IsAny<string>())).ReturnsAsync(customer);
            mockMapper.Setup(m => m.Map<IEnumerable<TransactionViewModel>>(It.IsAny<List<Transaction>>()))
                .Returns((new List<TransactionViewModel> { }));


             var controller = new CustomerController(customerAppService: mockService.Object, mapper: mockMapper.Object);

            // Act
            var result = await controller.GetTransactionsByCpf("5445");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);

            // Assert
            Assert.IsType<NotFoundObjectResult>(notFoundResult);
        }

        [Fact]
        public async Task GetTransactionsByCpf_ReturnOkObjectResult_When_ModelIsValid()
        {
            // Arrange
            var mockService = new Mock<ICustomerAppService>();
            var mockMapper = new Mock<IMapper>();

            var customer = new Customer(Guid.NewGuid().ToString(), "56544854698", "João", DateTime.Now);
            customer.Account = new Account("7127hs71hs17hs7217s2", 1232, "18187277163263");
            customer.Account.Transactions = new List<Transaction>()
            {
                new Transaction("h1y2hy31h2", TransactionType.Deposit, DateTime.Now, 12, "1231jjusju12j")
            };

            var transactionsViewModel = new List<TransactionViewModel>()
            {
                new TransactionViewModel()
                {
                    TransactionType = TransactionType.Deposit.ToString(),
                    Date = DateTime.Now,
                    Value = 12
                }
            };

            mockService.Setup(service => service.GetByCpf(It.IsAny<string>())).ReturnsAsync(customer);
            mockMapper.Setup(m => m.Map<IEnumerable<TransactionViewModel>>(customer.Account.Transactions)).Returns(transactionsViewModel);

            var controller = new CustomerController(customerAppService: mockService.Object, mapper: mockMapper.Object);

            // Act
            var result = await controller.GetTransactionsByCpf("5445");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<TransactionViewModel>>(okResult.Value);

            Assert.Equal(transactionsViewModel, returnValue);
        }
    }
}
