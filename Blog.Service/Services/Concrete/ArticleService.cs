using Blog.Entity.Entities;
using Blog.Service.Services.Abstracts;
using Blog.Data.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Entity.ViewModels.Articles;
using AutoMapper;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Blog.Service.Extensions;

namespace Blog.Service.Services.Concrete
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
        }

        public async Task CreateArticleAsync(ArticleAddVM articleAddVM)
        {
            //var userId = Guid.Parse("5988CE36-F81D-459F-B405-8CEC5CCBF841");

            var userId = _user.GetLoggedInUserId();
            var userEmail = _user.GetLoggedInEmail();

            var imageId = Guid.Parse("F71F4B9A-AA60-461D-B398-DE31001BF214");
            var article = new Article(articleAddVM.Title, articleAddVM.Content, userId, userEmail, articleAddVM.CategoryId, imageId);

            await unitOfWork.GetRepository<Article>().AddAsync(article);
            await unitOfWork.SaveAsync();
        }

        public async Task<List<ArticleVM>> GetAllArticlesWithCategoryNonDeletedAsync()
        {
            //GetRepository metodu ile tek bir metoddan generic şekilde içindeki tüm metodlara ulaşabildik.
            //Include kullanmadan istediklerimizi getirebildik func sayesinde.
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => !x.IsDeleted, x => x.Category);
            var map = mapper.Map<List<ArticleVM>>(articles);
            return map;
        }

        public async Task<ArticleVM> GetArticleWithCategoryNonDeletedAsync(Guid articleId)
        {
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleId, x => x.Category);
            var map = mapper.Map<ArticleVM>(article);
            return map;
        }

        public async Task<string> UpdateArticleAsync(ArticleUpdateVM articleUpdateVM)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleUpdateVM.Id, x => x.Category);

            article.Title = articleUpdateVM.Title;
            article.Content = articleUpdateVM.Content;
            article.CategoryId = articleUpdateVM.CategoryId;
            article.ModifiedDate = DateTime.Now;
            article.ModifiedBy = userEmail;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();

            return article.Title;
        }

        public async Task<string> SafeDeleteArticleAsync(Guid articleId)
        {
            var userEmail = _user.GetLoggedInEmail();

            var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);

            article.IsDeleted = true;
            article.DeletedDate = DateTime.Now;
            article.DeletedBy = userEmail;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();

            return article.Title;
        }
    }
}
