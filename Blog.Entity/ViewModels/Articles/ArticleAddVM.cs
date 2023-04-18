using Blog.Service.Services.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Entity.ViewModels.Articles
{
    public class ArticleAddVM
    {
        public string Title { get; set; }
        public string Content{ get; set; }
        public Guid CategoryId { get; set; }
        public IList<CategoryVM> Categories { get; set; }

    }
}
