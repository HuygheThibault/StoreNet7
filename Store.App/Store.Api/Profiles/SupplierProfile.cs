using AutoMapper;
using Store.Api.Models;
using Store.Shared.Dto;

namespace Store.Api.Profiles
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            this.CreateMap<Supplier, SupplierDto>().ReverseMap();
        }
    }
}
