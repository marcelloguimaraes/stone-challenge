using AutoMapper;
using StoneChallenge.Bank.Application.ViewModels;
using StoneChallenge.Bank.Domain.Models;

namespace StoneChallenge.Bank.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<AccountViewModel, Account>();
            CreateMap<CustomerViewModel, Customer>();
        }
    }
}
