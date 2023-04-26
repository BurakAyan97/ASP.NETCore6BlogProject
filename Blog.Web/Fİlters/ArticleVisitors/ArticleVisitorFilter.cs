using Blog.Data.Migrations;
using Blog.Data.UnitOfWorks;
using Blog.Entity.Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Blog.Web.Fİlters.ArticleVisitors
{
    public class ArticleVisitorFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork unitOfWork;

        public ArticleVisitorFilter(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //public bool Disable { get; set; }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //if (Disable) { return next(); }

            List<Visitor> visitors = unitOfWork.GetRepository<Visitor>().GetAllAsync().Result;
            //Giriş yapan kullanıcının IP adresini alır
            string getIp = context.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string getUserAgent = context.HttpContext.Request.Headers["User-Agent"];

            Visitor visitor = new(getIp, getUserAgent);

            if (visitors.Any(x => x.IpAddress == visitor.IpAddress))
            {
                return next();
            }
            else
            {
                unitOfWork.GetRepository<Visitor>().AddAsync(visitor);
                unitOfWork.Save();
            }
            return next();
        }
    }
}
