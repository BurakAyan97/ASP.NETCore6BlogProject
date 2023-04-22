using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Articles;
using Blog.Entity.ViewModels.Users;
using Blog.Service.Extensions;
using Blog.Service.Helpers.Images;
using Blog.Service.Services.Abstracts;
using Blog.Web.ResultMessages;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Data;
using System.Runtime.InteropServices;
using static Blog.Web.ResultMessages.Messages;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {

        private readonly IMapper mapper;
        private readonly IValidator<AppUser> validator;
        private readonly IToastNotification toast;
        private readonly IUserService userService;

        public UserController(IMapper mapper, IValidator<AppUser> validator, IToastNotification toast, IUserService userService)
        {
            this.mapper = mapper;
            this.validator = validator;
            this.toast = toast;
            this.userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await userService.GetAllUsersWithRoleAsync();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await userService.GetAllRolesAsync();
            return View(new UserAddVM { Roles = roles });
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserAddVM userAddVM)
        {
            var map = mapper.Map<AppUser>(userAddVM);
            var validation = await validator.ValidateAsync(map);
            var roles = await userService.GetAllRolesAsync();

            if (ModelState.IsValid)
            {
                var result = await userService.CreateUserAsync(userAddVM);
                if (result.Succeeded)
                {
                    toast.AddSuccessToastMessage(Messages.User.Add(userAddVM.Email), new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "User", new { Area = "Admin" });
                }
                else
                {
                    result.AddToIdentityModelState(this.ModelState);
                    validation.AddToModelState(this.ModelState);
                    return View(new UserAddVM { Roles = roles });
                }
            }
            return View(new UserAddVM { Roles = roles });
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid userId)
        {
            var user = await userService.GetAppUserByIdAsync(userId);
            var roles = await userService.GetAllRolesAsync();

            var map = mapper.Map<UserUpdateVM>(user);
            map.Roles = roles;

            return View(map);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateVM userUpdateVM)
        {
            var user = await userService.GetAppUserByIdAsync(userUpdateVM.Id);

            if (user != null)
            {
                var roles = await userService.GetAllRolesAsync();
                if (ModelState.IsValid)
                {
                    var map = mapper.Map(userUpdateVM, user);
                    var validation = await validator.ValidateAsync(map);

                    if (validation.IsValid)
                    {
                        user.UserName = userUpdateVM.Email;
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        var result = await userService.UpdateUserAsync(userUpdateVM);

                        if (result.Succeeded)
                        {
                            toast.AddSuccessToastMessage(Messages.User.Update(userUpdateVM.Email), new ToastrOptions { Title = "İşlem Başarılı" });
                            return RedirectToAction("Index", "User", new { Area = "Admin" });
                        }
                        else
                        {
                            result.AddToIdentityModelState(this.ModelState);
                            return View(new UserUpdateVM { Roles = roles });
                        }
                    }
                    else
                    {
                        validation.AddToModelState(this.ModelState);
                        return View(new UserUpdateVM { Roles = roles });
                    }
                }
            }
            return NotFound();
        }
        public async Task<IActionResult> Delete(Guid userid)
        {
            var result = await userService.DeleteUserAsync(userid);

            if (result.identityResult.Succeeded)
            {
                toast.AddSuccessToastMessage(Messages.User.Delete(result.email), new ToastrOptions { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else
                result.identityResult.AddToIdentityModelState(this.ModelState);

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profile = await userService.GetUserProfileAsync();
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileVM userProfileVM)
        {
            if (ModelState.IsValid)
            {
                var result = await userService.UserProfileUpdateAsync(userProfileVM);
                if (result)
                {
                    toast.AddSuccessToastMessage("Profil Güncelleme İşlemi Tamamlandı.", new ToastrOptions { Title = "İşlem Başarılı" });
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else
                {
                    var profile = await userService.GetUserProfileAsync();
                    toast.AddErrorToastMessage("Profil Güncelleme İşlemi Tamamlanamadı.", new ToastrOptions { Title = "İşlem Başarısız." });
                    return View(profile);
                }
            }
            return NotFound();
        }
    }
}
