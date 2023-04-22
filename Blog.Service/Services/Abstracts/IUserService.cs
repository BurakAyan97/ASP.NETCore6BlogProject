using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Abstracts
{
    public interface IUserService
    {
        Task<List<UserVM>> GetAllUsersWithRoleAsync();
        Task<List<AppRole>> GetAllRolesAsync();
        Task<IdentityResult> CreateUserAsync(UserAddVM userAddVM);
        Task<IdentityResult> UpdateUserAsync(UserUpdateVM userUpdateVM);
        Task<(IdentityResult identityResult, string? email)> DeleteUserAsync(Guid userId);//Tuple ile birden fazla nesne taşıyoruz ve item1,item2 gözükmesin diye isimlendiriyoruz.
        Task<AppUser> GetAppUserByIdAsync(Guid userId);
        Task<string> GetUserRoleAsync(AppUser user);
        Task<UserProfileVM> GetUserProfileAsync();
        Task<bool> UserProfileUpdateAsync(UserProfileVM userProfileVM);
    }
}
