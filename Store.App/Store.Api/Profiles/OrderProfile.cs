using AutoMapper;
using Store.Api.Models;
using Store.Shared.Dto;

namespace Store.Api.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            this.CreateMap<Order, OrderDto>().ReverseMap();
        }
    }
}
