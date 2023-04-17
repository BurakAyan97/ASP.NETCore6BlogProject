using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Data.Repositories.Abstracts;
using Blog.Entity.Entities;

namespace Blog.Data.UnitOfWorks
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        //Tüm repositoryleri tek tek almak yerine generic şekilde oluşturuyoruz.
        IRepository<T> GetRepository<T>() where T : class,IBaseEntity, new();
        Task<int> SaveAsync();
        int Save();
    }
}
