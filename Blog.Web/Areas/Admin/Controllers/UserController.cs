using AutoMapper;
using Blog.Data.UnitOfWorks;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Articles;
using Blog.Entity.ViewModels.Users;
using Blog.Service.Extensions;
using Blog.Service.Helpers.Images;
using Blog.Web.ResultMessages;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Data;
using System.Runtime.InteropServices;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IMapper mapper;
        private readonly IValidator<AppUser> validator;
        private readonly IToastNotification toast;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IImageHelper imageHelper;
        private readonly IUnitOfWork unitOfWork;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper, IValidator<AppUser> validator, IToastNotification toast, SignInManager<AppUser> signInManager, IImageHelper imageHelper, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.validator = validator;
            this.toast = toast;
            this.signInManager = signInManager;
            this.imageHelper = imageHelper;
            this.unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            var map = mapper.Map<List<UserVM>>(users);

            foreach (var user in map)
            {
                var findUser = await userManager.FindByIdAsync(user.Id.ToString());
                var role = string.Join("", await userManager.GetRolesAsync(findUser));
                //role liste olarak geldiği için stringe çevirmek gerekti.Çok rol olsa ile join ile yan yana yazılı string olurdu.
                user.Role = role;
            }
            return View(map);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(new UserAddVM { Roles = roles });
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserAddVM userAddVM)
        {
            var map = mapper.Map<AppUser>(userAddVM);
            var validation = await validator.ValidateAsync(map);
            var roles = await roleManager.Roles.ToListAsync();

            if (ModelState.IsValid)
            {
                map.UserName = userAddVM.Email;
                var result = await userManager.CreateAsync(map, string.IsNullOrEmpty(userAddVM.Password) ? "" : userAddVM.Password);
                if (result.Succeeded)
                {
                    var findRole = await roleManager.FindByIdAsync(userAddVM.RoleId.ToString());
                    await userManager.AddToRoleAsync(map, findRole.ToString());
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
            var user = await userManager.FindByIdAsync(userId.ToString());
            var roles = await roleManager.Roles.ToListAsync();

            var map = mapper.Map<UserUpdateVM>(user);
            map.Roles = roles;
            return View(map);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateVM userUpdateVM)
        {
            var user = await userManager.FindByIdAsync(userUpdateVM.Id.ToString());

            if (user == null)
            {
                var userRole = string.Join("", await userManager.GetRolesAsync(user));
                var roles = await roleManager.Roles.ToListAsync();
                if (ModelState.IsValid)
                {
                    var map = mapper.Map(userUpdateVM, user);
                    var validation = await validator.ValidateAsync(map);

                    if (validation.IsValid)
                    {
                        user.UserName = userUpdateVM.Email;
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        var result = await userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            await userManager.RemoveFromRoleAsync(user, userRole);
                            var findRole = await roleManager.FindByIdAsync(userUpdateVM.RoleId.ToString());
                            await userManager.AddToRoleAsync(user, findRole.Name);
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
            var user = await userManager.FindByIdAsync(userid.ToString());

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                toast.AddSuccessToastMessage(Messages.User.Delete(user.Email), new ToastrOptions { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "User", new { Area = "Admin" });
            }
            else
                result.AddToIdentityModelState(this.ModelState);

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            var getImage = await unitOfWork.GetRepository<AppUser>().GetAsync(x => x.Id == user.Id, x => x.Image);
            var map = mapper.Map<UserProfileVM>(user);
            map.Image.FileName = getImage.Image.FileName;
            return View(map);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileVM userProfileVM)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {
                //Kullanıcının mevcut şifresinin doğru mu yanlış diye bakıp bool döndürüyor.
                var isVerified = await userManager.CheckPasswordAsync(user, userProfileVM.CurrentPassword);
                if (isVerified && userProfileVM.NewPassword is not null && userProfileVM.Photo is not null)
                {
                    var result = await userManager.ChangePasswordAsync(user, userProfileVM.CurrentPassword, userProfileVM.NewPassword);
                    if (result.Succeeded)
                    {
                        await userManager.UpdateSecurityStampAsync(user);
                        await signInManager.SignOutAsync();
                        await signInManager.PasswordSignInAsync(user, userProfileVM.NewPassword, true, false);

                        user.FirstName = userProfileVM.FirstName;
                        user.LastName = userProfileVM.LastName;
                        user.PhoneNumber = userProfileVM.PhoneNumber;

                        var imageUpload = await imageHelper.Upload($"{userProfileVM.FirstName} {userProfileVM.LastName}", userProfileVM.Photo, Entity.Enums.ImageType.User);
                        Image image = new(imageUpload.FullName, userProfileVM.Photo.ContentType, user.Email);
                        await unitOfWork.GetRepository<Image>().AddAsync(image);

                        user.ImageId = image.Id;

                        await userManager.UpdateAsync(user);

                        await unitOfWork.SaveAsync();

                        toast.AddSuccessToastMessage("Şifreniz ve bilgileriniz başarıyla değiştirilmiştir.");
                        return View();
                    }
                    else
                        result.AddToIdentityModelState(ModelState); return View();
                }
                else if (isVerified && userProfileVM.Photo is not null)
                {
                    await userManager.GetSecurityStampAsync(user);
                    user.FirstName = userProfileVM.FirstName;
                    user.LastName = userProfileVM.LastName;
                    user.PhoneNumber = userProfileVM.PhoneNumber;

                    var imageUpload = await imageHelper.Upload($"{userProfileVM.FirstName} {userProfileVM.LastName}", userProfileVM.Photo, Entity.Enums.ImageType.User);
                    Image image = new(imageUpload.FullName, userProfileVM.Photo.ContentType, user.Email);
                    await unitOfWork.GetRepository<Image>().AddAsync(image);

                    user.ImageId = image.Id;

                    await userManager.UpdateAsync(user);
                    await unitOfWork.SaveAsync();
                    toast.AddSuccessToastMessage("Bilgileriniz başarıyla değiştirilmiştir.");
                    return View();
                }
                else
                    toast.AddErrorToastMessage("Bilgileriniz güncellenirken bir hata oluştu."); return View();
            }
            return View();
        }
    }
}
