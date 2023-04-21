using AutoMapper;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Categories;
using Blog.Entity.ViewModels.Users;
using Blog.Service.Services.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.AutoMapper.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserVM>().ReverseMap();
            CreateMap<AppUser, UserAddVM>().ReverseMap();
            CreateMap<AppUser, UserUpdateVM>().ReverseMap();

        }
    }
}
