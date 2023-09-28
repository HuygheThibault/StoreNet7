using AutoMapper;
using Store.Api.Models;
using Store.Shared.Dto;

namespace Store.Api.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            this.CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
