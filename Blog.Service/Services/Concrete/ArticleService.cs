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

namespace Blog.Service.Services.Concrete
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task CreateArticleAsync(ArticleAddVM articleAddVM)
        {
            var userId = Guid.Parse("5988CE36-F81D-459F-B405-8CEC5CCBF841");
            var article = new Article
            {
                Title = articleAddVM.Title,
                Content = articleAddVM.Content,
                CategoryId = articleAddVM.CategoryId,
                UserId = userId
            };

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

        public async Task UpdateArticleAsync(ArticleUpdateVM articleUpdateVM)
        {
            var article = await unitOfWork.GetRepository<Article>().GetAsync(x => !x.IsDeleted && x.Id == articleUpdateVM.Id, x => x.Category);
            
            article.Title = articleUpdateVM.Title;
            article.Content = articleUpdateVM.Content;
            article.CategoryId = articleUpdateVM.CategoryId;

            await unitOfWork.GetRepository<Article>().UpdateAsync(article);
            await unitOfWork.SaveAsync();
        }
    }
}
