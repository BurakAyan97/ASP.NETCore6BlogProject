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
        public async Task<List<ArticleVM>> GetAllArticlesAsync()
        {
            //GetRepository metodu ile tek bir metoddan generic şekilde içindeki tüm metodlara ulaşabildik.
            var articles = await unitOfWork.GetRepository<Article>().GetAllAsync();
            var map = mapper.Map<List<ArticleVM>>(articles);
            return map;
        }
    }
}
