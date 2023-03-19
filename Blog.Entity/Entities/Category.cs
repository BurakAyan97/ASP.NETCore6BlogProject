using Blog.Core.Entities;

namespace Blog.Entity.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; }

    }
}