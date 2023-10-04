using AutoMapper;
using Store.Api.Models;
using Store.Shared.Dto;

namespace Store.Api.Profiles
{
    public class OrderLineProfile : Profile
    {
        public OrderLineProfile()
        {
            this.CreateMap<OrderLine, OrderLineDto>().ReverseMap();
        }
    }
}
