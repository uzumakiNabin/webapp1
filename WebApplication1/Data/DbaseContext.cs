using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class DbaseContext : IdentityDbContext
    {
        public DbaseContext(DbContextOptions<DbaseContext> options)
            : base(options)
        {
        }

        public DbSet<Department>? Department {get; set;}
    }
}
