using Blog.Core.Entities;

namespace Blog.Entity.Entities
{
    public class Category : BaseEntity
    {
        public Category()
        {
            
        }
        public Category(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; }

    }
}