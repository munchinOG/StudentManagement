using Microsoft.EntityFrameworkCore;

namespace Student.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options )
            : base( options )
        {

        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            modelBuilder.Seed();
        }
    }
}
