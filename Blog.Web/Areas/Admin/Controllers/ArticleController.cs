using AutoMapper;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Articles;
using Blog.Service.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, IMapper mapper)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
            return View(articles);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return View(new ArticleAddVM { Categories = categories });
        }
        [HttpPost]
        public async Task<IActionResult> Add(ArticleAddVM articleAddVM)
        {
            await _articleService.CreateArticleAsync(articleAddVM);
            RedirectToAction("Index", "Article", new { Area = "Admin" });

            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            return View(new ArticleAddVM { Categories = categories });
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid articleId)
        {
            var article = await _articleService.GetArticleWithCategoryNonDeletedAsync(articleId);
            var categories = await _categoryService.GetAllCategoriesNonDeleted();

            var articleUpdateVM = _mapper.Map<ArticleAddVM>(article);
            articleUpdateVM.Categories = categories;

            return View(articleUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ArticleUpdateVM articleUpdateVM)
        {



            //var article = await _articleService.GetArticleWithCategoryNonDeletedAsync(articleId);
            //var categories = await _categoryService.GetAllCategoriesNonDeleted();

            //var articleUpdateVM = _mapper.Map<ArticleAddVM>(article);
            //articleUpdateVM.Categories = categories;

            //return View(articleUpdateVM);
        }
    }
}
