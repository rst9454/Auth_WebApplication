using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth_WebApplication.Data
{
    public class ApplicationContext:IdentityDbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
