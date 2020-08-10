using System.Collections.Generic;

namespace Student.Models
{
    public interface IStudentRepository
    {
        Student GetStudent( int Id );
        IEnumerable<Student> GetAllStudents( );
    }
}
