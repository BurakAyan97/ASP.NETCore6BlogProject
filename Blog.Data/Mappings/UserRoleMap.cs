using Blog.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Data.Mappings
{
    public class UserRoleMap : IEntityTypeConfiguration<AppUserRole>
    {
        public void Configure(EntityTypeBuilder<AppUserRole> builder)
        {
            builder.HasKey(r => new { r.UserId, r.RoleId });

            // Maps to the AspNetUserRoles table
            builder.ToTable("AspNetUserRoles");

            builder.HasData(new AppUserRole
            {
                UserId = Guid.Parse("5988CE36-F81D-459F-B405-8CEC5CCBF841"),
                RoleId = Guid.Parse("38816B98-1532-4ADF-BF0B-87A22A853214")
            },
            new AppUserRole
            {
                UserId = Guid.Parse("0F735C2F-A739-4FFF-A9FA-E0132AD614BE"),
                RoleId = Guid.Parse("6EC4F3C0-D90F-4C82-8903-D66508FA1223")
            });
        }
    }
}
