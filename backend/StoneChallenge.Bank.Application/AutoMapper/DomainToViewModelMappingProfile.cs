using AutoMapper;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;
using System.Collections.Generic;

namespace StoneChallenge.Bank.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Account, AccountViewModel>();
            CreateMap<Account, AccountListViewModel>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name));

            CreateMap<Customer, CustomerViewModel>();

            CreateMap<Transaction, TransactionViewModel>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => TransactionResolver(src.TransactionType)))
                .ForMember(dest => dest.DateFormatted, opt => opt.MapFrom(src => src.Date.ToString("dd/MM/yyyy HH:mm:ss")));
        }
        private string TransactionResolver(TransactionType transactionType)
        {
            string resolved = "";
            switch (transactionType)
            {
                case TransactionType.Deposit:
                    resolved = "Depósito";
                    break;
                case TransactionType.Withdraw:
                    resolved = "Saque";
                    break;
                case TransactionType.Transfer:
                    resolved = "Transferência";
                    break;
                default:
                    break;
            }
            return resolved;
        }
    }
}
