using AutoMapper;
using Store.Api.Models;
using Store.Shared.Dto;

namespace Store.Api.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            this.CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
