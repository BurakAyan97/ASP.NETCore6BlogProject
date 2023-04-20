using AutoMapper;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Categories;
using Blog.Service.Services.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.AutoMapper.Categories
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CategoryVM, Category>().ReverseMap();
            CreateMap<CategoryAddVM, Category>().ReverseMap();
            CreateMap<CategoryUpdateVM, Category>().ReverseMap();
        }
    }
}
