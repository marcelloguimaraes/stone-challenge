using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StoneChallenge.Bank.API.Controllers;
using StoneChallenge.Bank.Application.Interfaces;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static StoneChallenge.Bank.Application.ViewModels.AuthViewModel;

namespace StoneChallenge.Bank.Tests
{
    public class AccountControllerTests
    {
        #region Transfer

        [Fact]
        public async Task Transfer_Given_InvalidModelState_Then_ReturnBadRequestObjectResult()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();
            var controller = new AccountController(mockService.Object);
            controller.ModelState.AddModelError("TransferViewModelInvalid", "Informe um modelo válido");

            //Act
            var result = await controller.Transfer(transferViewModel: null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Transfer_Given_TransferingToSameAccount_Then_ReturnBadRequestObjectResult()
        {
            // Arrange
            //var mockService = new Mock<IAccountAppService>();
            var controller = new AccountController(null);
            //controller.ModelState.AddModelError("TransferViewModelInvalid", "Informe um modelo válido");

            var transferViewModel = new TransferViewModel()
            {
                SourceAccount = new SourceAccountViewModel() { AccountNumber = 1},
                TargetAccount = new TargetAccountViewModel() { AccountNumber = 1}
            };

            //Act
            var result = await controller.Transfer(transferViewModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Não é permitido transferir para mesma conta", badRequestResult.Value);
        }

        [Fact]
        public async Task Transfer_ReturnOkObjectResult_When_ModelIsValid()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockService.Object, mapper: mockMapper.Object);

            var sourceAccount = new Account("47a77e3a-4db7-44bc-aa01-bcab6b58e191",
                                            3032,
                                            "b6f5a05b-a466-4bcc-977a-507e9745a227")
            { Balance = 50 };

            var targetAccount = new Account("d3067022-5fab-4a42-9c80-6dab8b231a21",
                                            3031,
                                            "b84385d7-6b8c-459e-bf29-db46857fd40c")
            { Balance = 50 };

            var transferViewModel = new TransferViewModel()
            {
                SourceAccount = new SourceAccountViewModel() { AccountNumber = sourceAccount.AccountNumber, Agency = 3032 },
                TargetAccount = new TargetAccountViewModel() { AccountNumber = targetAccount.AccountNumber, Agency = 3031 },
                Value = 10
            };

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        sourceAccount.AccountNumber,
                        sourceAccount.Agency
                    )).ReturnsAsync(sourceAccount);

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        targetAccount.AccountNumber,
                        targetAccount.Agency
                    )).ReturnsAsync(targetAccount);

            var sourceAccountExpected = new AccountListViewModel()
            {
                AccountNumber = sourceAccount.AccountNumber,
                Agency = sourceAccount.Agency,
                Balance = 39 // 50 - (10+1(taxa))
            };

            var targetAccountExpected = new AccountListViewModel()
            {
                AccountNumber = sourceAccount.AccountNumber,
                Agency = sourceAccount.Agency,
                Balance = 60 // 50 + 10(deposito)
            };

            mockMapper.Setup(m => m.Map<Account, AccountListViewModel>(sourceAccount)).Returns(sourceAccountExpected);
            mockMapper.Setup(m => m.Map<Account, AccountListViewModel>(targetAccount)).Returns(targetAccountExpected);

            //Act
            var result = await controller.Transfer(transferViewModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TransferListViewModel>(okResult.Value);

            Assert.Equal(sourceAccountExpected, returnValue.SourceAccount);
            Assert.Equal(targetAccountExpected, returnValue.TargetAccount);
        }

        [Fact]
        public async Task Transfer_Given_InvalidSourceAccount_Then_ReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();

            var invalidTransferViewModel = new TransferViewModel()
            {
                SourceAccount = new SourceAccountViewModel() { AccountNumber = 0, Agency = 0 },
                TargetAccount = new TargetAccountViewModel() { AccountNumber = 123, Agency = 123 },
            };

            var sourceAccount = invalidTransferViewModel.SourceAccount;

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        sourceAccount.AccountNumber,
                        sourceAccount.Agency
                    )).ReturnsAsync((Account)null);

            var controller = new AccountController(mockService.Object);

            // Act
            var result = await controller.Transfer(invalidTransferViewModel);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Transfer_Given_InvalidTargetAccount_Then_ReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();


            var invalidTransferViewModel = new TransferViewModel()
            {
                SourceAccount = new SourceAccountViewModel() { AccountNumber = 348710, Agency = 3032 },
                TargetAccount = new TargetAccountViewModel() { AccountNumber = 0, Agency = 0 }
            };


            var (sourceAccount, targetAccount) = (invalidTransferViewModel.SourceAccount, invalidTransferViewModel.TargetAccount);

            var account = new Account("b9def779-e3f9-4813-b789-3fe7c1306663",
                                      sourceAccount.Agency,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227");

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        sourceAccount.AccountNumber,
                        sourceAccount.Agency
                    )).ReturnsAsync(account);

            var controller = new AccountController(mockService.Object);

            // Act
            var result = await controller.Transfer(invalidTransferViewModel);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion

        #region Withdraw
        [Fact]
        public async Task Withdraw_Given_InvalidModelState_Then_ReturnBadRequestObjectResult()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();
            var controller = new AccountController(mockService.Object);
            controller.ModelState.AddModelError("WithdrawViewModelInvalid", "Informe um modelo válido");

            //Act
            var result = await controller.Withdraw(withdrawViewModel: null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Withdraw_Given_InvalidAccount_Then_ReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();

            var invalidWithdrawViewModel = new WithdrawViewModel()
            {
                AccountNumber = 0,
                Agency = 0
            };

            var sourceAccount = invalidWithdrawViewModel.AccountNumber;

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        invalidWithdrawViewModel.AccountNumber,
                        invalidWithdrawViewModel.Agency
                    )).ReturnsAsync((Account)null);

            var controller = new AccountController(mockService.Object);

            // Act
            var result = await controller.Withdraw(invalidWithdrawViewModel);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Withdraw_ReturnOkObjectResult_When_ModelIsValid()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockService.Object, mapper: mockMapper.Object);

            var account = new Account("47a77e3a-4db7-44bc-aa01-bcab6b58e191",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227");

            var withdrawViewModel = new WithdrawViewModel()
            {
                AccountNumber = 348710,
                Agency = 3032,
                Value = 10
            };

            var accountListViewModel = new AccountListViewModel()
            {
                AccountNumber = account.AccountNumber,
                Agency = account.Agency,
                CustomerName = "Marcello",
                Balance = 9.9
            };

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )).ReturnsAsync(account);

            mockMapper.Setup(m => m.Map<AccountListViewModel>(account)).Returns(accountListViewModel);

            //Act
            var result = await controller.Withdraw(withdrawViewModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AccountListViewModel>(okResult.Value);

            Assert.Equal(accountListViewModel, returnValue);

        }
        #endregion

        #region Deposit
        [Fact]
        public async Task Deposit_Given_InvalidModelState_Then_ReturnBadRequestObjectResult()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();
            var controller = new AccountController(mockService.Object);
            controller.ModelState.AddModelError("DepositViewModelInvalid", "Informe um modelo válido");

            //Act
            var result = await controller.Deposit(depositViewModel: null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Deposit_Given_InvalidAccount_Then_ReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();

            var invalidDepositViewModel = new DepositViewModel()
            {
                AccountNumber = 0,
                Agency = 0
            };

            var sourceAccount = invalidDepositViewModel.AccountNumber;

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        invalidDepositViewModel.AccountNumber,
                        invalidDepositViewModel.Agency
                    )).ReturnsAsync((Account)null);

            var controller = new AccountController(mockService.Object);

            // Act
            var result = await controller.Deposit(invalidDepositViewModel);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Deposit_ReturnOkObjectResult_When_ModelIsValid()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();
            var mockMapper = new Mock<IMapper>();
            var controller = new AccountController(mockService.Object, mapper: mockMapper.Object);

            var account = new Account("47a77e3a-4db7-44bc-aa01-bcab6b58e191",
                                      3032,
                                      "b6f5a05b-a466-4bcc-977a-507e9745a227");

            var accountListViewModel = new AccountListViewModel()
            {
                AccountNumber = account.AccountNumber,
                Agency = account.Agency,
                CustomerName = "Marcello",
                Balance = 9.9
            };

            var depositViewModel = new DepositViewModel()
            {
                AccountNumber = 348710,
                Agency = 3032,
                Value = 10
            };

            mockMapper.Setup(m => m.Map<AccountListViewModel>(account)).Returns(accountListViewModel);

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        It.IsAny<int>(),
                        It.IsAny<int>()
                    )).ReturnsAsync(account);

            mockService.Setup(service => service.Deposit(It.IsAny<Account>(), It.IsAny<double>()));
            //Act
            var result = await controller.Deposit(depositViewModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AccountListViewModel>(okResult.Value);

            Assert.Equal(accountListViewModel, returnValue);

        }
        #endregion

        #region Transactions
        [Fact]
        public async Task GetTransactionsByAccountNumberAndAgency_Given_InvalidModelState_Then_ReturnBadRequestObjectResult()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();
            var controller = new AccountController(mockService.Object);
            controller.ModelState.AddModelError("SourceAccountViewModel", "Informe um modelo válido");

            //Act
            var result = await controller.GetTransactionsByAccountNumberAndAgency(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task GetTransactionsByAccountNumberAndAgency_Given_InvalidAccount_Then_ReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();

            var invalidSourceAccountViewModel = new SourceAccountViewModel()
            {
                AccountNumber = 0,
                Agency = 0
            };

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        invalidSourceAccountViewModel.AccountNumber,
                        invalidSourceAccountViewModel.Agency
                    )).ReturnsAsync((Account)null);

            var controller = new AccountController(mockService.Object);

            // Act
            var result = await controller.GetTransactionsByAccountNumberAndAgency(invalidSourceAccountViewModel);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTransactionsByAccountNumberAndAgency_Given_NoneTransactionsForAccount_Then_ReturnNotFound()
        {
            // Arrange
            var mockService = new Mock<IAccountAppService>();

            var sourceAccountViewModel = new SourceAccountViewModel()
            {
                AccountNumber = 12232,
                Agency = 1232
            };

            var account = new Account("7127hs71hs17hs7217s2", 1232, "18187277163263");

            mockService.Setup(service => service.GetByAccountNumberAndAgencyAsync(
                        account.AccountNumber,
                        account.Agency
                    )).ReturnsAsync(account);

            var controller = new AccountController(mockService.Object);

            // Act
            var result = await controller.GetTransactionsByAccountNumberAndAgency(sourceAccountViewModel);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        #endregion

    }
}