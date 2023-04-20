using Blog.Entity.Entities;
using Blog.Entity.ViewModels.Categories;
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
        Task<List<CategoryVM>> GetAllCategoriesNonDeleted();
        Task CreateCategoryAsync(CategoryAddVM categoryAddVM);
        Task<Category> GetCategoryByGuid(Guid id);
        Task<string> UpdateCategoryAsync(CategoryUpdateVM categoryUpdateVM);
        Task<string> SafeDeleteCategoryAsync(Guid categoryId);
        Task<string> UndoDeleteCategoryAsync(Guid categoryId);
    }
}
