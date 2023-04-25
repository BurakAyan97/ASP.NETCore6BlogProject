using Blog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Entity.Entities
{
    public class ArticleVisitor:IBaseEntity
    {
        public ArticleVisitor()
        {

        }

        public ArticleVisitor(Guid articleId, int visitorId)
        {
            ArticleId = articleId;
            VisitorId = visitorId;
        }

        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }
    }
}
