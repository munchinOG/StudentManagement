using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Student.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options )
            : base( options )
        {

        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            base.OnModelCreating( modelBuilder );
            modelBuilder.Seed();

            foreach(var foreigkey in modelBuilder.Model.GetEntityTypes()
                .SelectMany( e => e.GetForeignKeys() ))
            {
                foreigkey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
