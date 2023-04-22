using Blog.Entity.Entities;
using Blog.Service.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IDashboardService dashboardService;

        public HomeController(IArticleService articleService,IDashboardService dashboardService)
        {
            _articleService = articleService;
            this.dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesWithCategoryNonDeletedAsync();
            var result = await dashboardService.GetYearlyArticleCounts();
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> YearlyArticleCount()
        {
            var count = await dashboardService.GetYearlyArticleCounts();
            return Json(JsonConvert.SerializeObject(count));
        }
    }
}
