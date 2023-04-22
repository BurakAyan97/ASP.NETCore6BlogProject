using Blog.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Entity.Entities
{
    public class AppUser:IdentityUser<Guid>,IBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid ImageId { get; set; } = Guid.Parse("F71F4B9A-AA60-461D-B398-DE31001BF214");
        public Image Image { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
