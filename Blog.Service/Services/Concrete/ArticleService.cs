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
using Blog.Service.Helpers.Images;
using Blog.Entity.Enums;

namespace Blog.Service.Services.Concrete
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ClaimsPrincipal _user;
        private readonly IImageHelper _imageHelper;
        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IImageHelper imageHelper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            _user = httpContextAccessor.HttpContext.User;
            _imageHelper = imageHelper;
        }

        public async Task CreateArticleAsync(ArticleAddVM articleAddVM)
        {
            var userId = _user.GetLoggedInUserId();
            var userEmail = _user.GetLoggedInEmail();

            var imageUpload = await _imageHelper.Upload(articleAddVM.Title, articleAddVM.Photo, Entity.Enums.ImageType.Post);
            Image image = new(imageUpload.FullName, articleAddVM.Photo.ContentType, userEmail);
            await unitOfWork.GetRepository<Image>().AddAsync(image);


            var article = new Article(articleAddVM.Title, articleAddVM.Content, userId, userEmail, articleAddVM.CategoryId, image.Id);

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
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleId, x => x.Category, i => i.Image);
            var map = mapper.Map<ArticleVM>(article);
            return map;
        }

        public async Task<string> UpdateArticleAsync(ArticleUpdateVM articleUpdateVM)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleUpdateVM.Id, x => x.Category, i => i.Image);

            if (articleUpdateVM.Photo != null)
            {
                _imageHelper.Delete(article.Image.FileName);

                var imageUpload = await _imageHelper.Upload(articleUpdateVM.Title, articleUpdateVM.Photo, ImageType.Post);
                Image image = new(imageUpload.FullName, articleUpdateVM.Photo.ContentType, userEmail);
                await unitOfWork.GetRepository<Image>().AddAsync(image);

                article.ImageId = image.Id;
            }

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

        public async Task<List<ArticleVM>> GetAllArticlesWithCategoryDeletedAsync()
        {
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync(x => x.IsDeleted, x => x.Category);
            var map = mapper.Map<List<ArticleVM>>(articles);

            return map;
        }

        public async Task<string> UndoDeleteArticleAsync(Guid articleId)
        {
            var userEmail = _user.GetLoggedInEmail();
            var article = await unitOfWork.GetRepository<Article>().GetByGuidAsync(articleId);

            article.IsDeleted = false;
            article.DeletedDate = null;
            article.DeletedBy = null;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();

            return article.Title;
        }
    }
}
