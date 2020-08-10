using System.Collections.Generic;
using System.Linq;

namespace Student.Models
{
    public class MockStudentRepository : IStudentRepository
    {
        private readonly List<Student> _studentList;

        public MockStudentRepository( )
        {
            _studentList = new List<Student>()
            {
                new Student(){Id = 1, Name = "Oguns", Department = "Computer-Sci", Email = "oguns@student.com"},
                new Student(){Id = 2, Name = "Kiki", Department = "Act", Email = "kiki@student.com"},
                new Student(){Id = 1, Name = "Ruka", Department = "English", Email = "ruka@student.com"}
            };
        }

        public IEnumerable<Student> GetAllStudents( )
        {
            return _studentList;
        }

        public Student GetStudent( int Id )
        {
            return _studentList.FirstOrDefault( e => e.Id == Id );
        }
    }
}
