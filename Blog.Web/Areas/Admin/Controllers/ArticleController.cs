﻿using AutoMapper;
using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Articles;
using Blog.Service.Services.Abstracts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IValidator<Article> _validator;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, IMapper mapper, IValidator<Article> validator)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _mapper = mapper;
            _validator = validator;
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
            var map = _mapper.Map<Article>(articleAddVM);
            var result = await _validator.ValidateAsync(map);

            if (result.IsValid)
            {
                await _articleService.CreateArticleAsync(articleAddVM);
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
            var map = _mapper.Map<Article>(articleUpdateVM);
            var result = await _validator.ValidateAsync(map);

            if (result.IsValid)
            {
                await _articleService.UpdateArticleAsync(articleUpdateVM);

            }
            else
            {
                result.AddToModelState(this.ModelState);
            }

            var categories = await _categoryService.GetAllCategoriesNonDeleted();
            articleUpdateVM.Categories = categories;
            return View(articleUpdateVM);
        }

        public async Task<IActionResult> Delete(Guid articleId)
        {
            await _articleService.SafeDeleteArticleAsync(articleId);

            return RedirectToAction("Index", "Article", new { Area = "Admin" });
        }
    }
}