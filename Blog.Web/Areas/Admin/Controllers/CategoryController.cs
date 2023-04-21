using AutoMapper;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Categories;
using Blog.Service.Services.Abstracts;
using Blog.Web.ResultMessages;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IValidator<Category> _validator;
        private readonly IMapper _mapper;
        private readonly IToastNotification _toast;
        public CategoryController(ICategoryService categoryService, IValidator<Category> validator, IMapper mapper, IToastNotification toast)
        {
            this.categoryService = categoryService;
            _validator = validator;
            _mapper = mapper;
            _toast = toast;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllCategoriesNonDeleted();
            return View(categories);
        }

        public async Task<IActionResult> DeletedCategory()
        {
            var categories = await categoryService.GetAllCategoriesDeleted();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddVM categoryAddVM)
        {
            var map = _mapper.Map<Category>(categoryAddVM);
            var result = await _validator.ValidateAsync(map);

            if (result.IsValid)
            {
                await categoryService.CreateCategoryAsync(categoryAddVM);
                _toast.AddSuccessToastMessage(Messages.Category.Add(categoryAddVM.Name), new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(this.ModelState);
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddWithAjax([FromBody] CategoryAddVM categoryAddVM)
        {
            var map = _mapper.Map<Category>(categoryAddVM);
            var result = await _validator.ValidateAsync(map);

            if (result.IsValid)
            {
                await categoryService.CreateCategoryAsync(categoryAddVM);
                _toast.AddSuccessToastMessage(Messages.Category.Add(categoryAddVM.Name), new ToastrOptions { Title = "İşlem Başarılı" });

                return Json(Messages.Category.Add(categoryAddVM.Name));
            }
            else
            {
                _toast.AddErrorToastMessage(result.Errors.First().ErrorMessage, new ToastrOptions { Title = "İşlem Başarısız" });
                return Json(result.Errors.First().ErrorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid categoryId)
        {
            var category = await categoryService.GetCategoryByGuid(categoryId);
            var map = _mapper.Map<Category, CategoryUpdateVM>(category);
            return View(map);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateVM categoryUpdateVM)
        {
            var map = _mapper.Map<Category>(categoryUpdateVM);
            var result = await _validator.ValidateAsync(map);
            if (result.IsValid)
            {
                var name = await categoryService.UpdateCategoryAsync(categoryUpdateVM);
                _toast.AddSuccessToastMessage(Messages.Category.Update(name), new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction("Index", "Category", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(this.ModelState);
                return View();
            }
        }

        public async Task<IActionResult> Delete(Guid categoryId)
        {
            var name = await categoryService.SafeDeleteCategoryAsync(categoryId);
            _toast.AddSuccessToastMessage(Messages.Category.Delete(name), new ToastrOptions() { Title = "İşlem Başarılı" });

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }
        public async Task<IActionResult> UndoDelete(Guid categoryId)
        {
            var name = await categoryService.UndoDeleteCategoryAsync(categoryId);
            _toast.AddSuccessToastMessage(Messages.Category.UndoDelete(name), new ToastrOptions() { Title = "İşlem Başarılı" });

            return RedirectToAction("Index", "Category", new { Area = "Admin" });
        }
    }
}
