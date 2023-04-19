using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Abstracts
{
    public interface IArticleService
    {
        Task<List<ArticleVM>> GetAllArticlesWithCategoryNonDeletedAsync();
        Task<ArticleVM> GetArticleWithCategoryNonDeletedAsync(Guid articleId);
        Task CreateArticleAsync(ArticleAddVM articleAddVM);
        Task<string> UpdateArticleAsync(ArticleUpdateVM articleUpdateVM);
        Task<string> SafeDeleteArticleAsync(Guid articleId);
    }
}
