using System.Collections.Generic;

namespace Student.Models
{
    public interface IStudentRepository
    {
        Student GetStudent( int id );
        IEnumerable<Student> GetAllStudent( );
        Student Add( Student student );
        Student Update( Student studentChanges );
        Student Delete( int id );
    }
}
