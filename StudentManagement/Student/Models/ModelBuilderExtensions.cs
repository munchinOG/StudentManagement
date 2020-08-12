using Microsoft.EntityFrameworkCore;

namespace Student.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed( this ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Name = "Munchin",
                    Department = Dept.ComputerSci,
                    Email = "munchin@ut.com"
                },
                new Student
                {
                    Id = 2,
                    Name = "Tom",
                    Department = Dept.Act,
                    Email = "tom@ut.com"
                }
            );
        }
    }
}
