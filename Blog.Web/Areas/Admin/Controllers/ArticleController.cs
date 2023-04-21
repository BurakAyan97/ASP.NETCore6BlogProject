using AutoMapper;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Articles;
using Blog.Service.Services.Abstracts;
using Blog.Web.Consts;
using Blog.Web.ResultMessages;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IValidator<Article> _validator;
        private readonly IToastNotification _toast;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, IMapper mapper, IValidator<Article> validator, IToastNotification toast)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _validator = validator;
            _toast = toast;
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin},{RoleConsts.User}")]
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
            return View(articles);
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin}")]
        public async Task<IActionResult> DeletedArticle()
        {
            var articles = await _articleService.GetAllArticlesWithCategoryDeletedAsync();
            return View(articles);
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin}")]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return View(new ArticleAddVM { Categories = categories });
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin}")]
        public async Task<IActionResult> Add(ArticleAddVM articleAddVM)
        {
            var map = _mapper.Map<Article>(articleAddVM);
            var result = await _validator.ValidateAsync(map);

            if (result.IsValid)
            {
                await _articleService.CreateArticleAsync(articleAddVM);
                _toast.AddSuccessToastMessage(Messages.Article.Add(articleAddVM.Title), new ToastrOptions { Title = "Başarılı!" });
                return RedirectToAction("Index", "Article", new { Area = "Admin" });
            }
            else
            {
                result.AddToModelState(this.ModelState);

            }
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return View(new ArticleAddVM { Categories = categories });
        }

        [HttpGet]
        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin}")]
        public async Task<IActionResult> Update(Guid articleId)
        {
            var article = await _articleService.GetArticleWithCategoryNonDeletedAsync(articleId);
            var categories = await _categoryService.GetAllCategoriesNonDeleted();

            var articleUpdateVM = _mapper.Map<ArticleAddVM>(article);
            articleUpdateVM.Categories = categories;

            return View(articleUpdateVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin}")]
        public async Task<IActionResult> Update(ArticleUpdateVM articleUpdateVM)
        {
            var map = _mapper.Map<Article>(articleUpdateVM);
            var result = await _validator.ValidateAsync(map);

            if (result.IsValid)
            {
                var title = await _articleService.UpdateArticleAsync(articleUpdateVM);
                _toast.AddSuccessToastMessage(Messages.Article.Update(title), new ToastrOptions() { Title = "İşlem Başarılı" });
                return RedirectToAction("Index", "Article", new { Area = "Admin" });


            }
            else
            {
                result.AddToModelState(this.ModelState);
            }

            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            articleUpdateVM.Categories = categories;
            return View(articleUpdateVM);
        }

        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin}")]
        public async Task<IActionResult> Delete(Guid articleId)
        {
            var title = await _articleService.SafeDeleteArticleAsync(articleId);

            _toast.AddSuccessToastMessage(Messages.Article.Delete(title), new ToastrOptions() { Title = "İşlem Başarılı" });

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }

        [Authorize(Roles = $"{RoleConsts.Admin},{RoleConsts.SuperAdmin}")]
        public async Task<IActionResult> UndoDelete(Guid articleId)
        {
            var title = await _articleService.UndoDeleteArticleAsync(articleId);

            _toast.AddSuccessToastMessage(Messages.Article.UndoDelete(title), new ToastrOptions() { Title = "İşlem Başarılı" });

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }

    }
}
