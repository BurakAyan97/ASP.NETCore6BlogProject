using Blog.Service.Services.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Service.Services.Abstracts
{
    public interface ICategoryService
    {
        public Task<List<CategoryVM>> GetAllCategoriesNonDeleted();
    }
}
