using Microsoft.EntityFrameworkCore;
using Weather1.Models;

namespace Weather1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Observation> Observations { get; set; }
    }
}