using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Student.Models
{
    public class SqlStudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SqlStudentRepository> _logger;

        public SqlStudentRepository( ApplicationDbContext context,
            ILogger<SqlStudentRepository> logger )
        {
            _context = context;
            _logger = logger;
        }

        public Student Add( Student student )
        {
            _context.Students.Add( student );
            _context.SaveChanges();
            return student;
        }

        public Student Delete( int id )
        {
            Student student = _context.Students.Find( id );
            if(student != null)
            {
                _context.Students.Remove( student );
                _context.SaveChanges();
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudent( )
        {
            return _context.Students;
        }

        public Student GetStudent( int id )
        {
            _logger.LogTrace( "Trace Log" );
            _logger.LogDebug( "Debug Log" );
            _logger.LogInformation( "Information Log" );
            _logger.LogWarning( "Warning Log" );
            _logger.LogError( "Error Log" );
            _logger.LogCritical( "Critical Log" );

            return _context.Students.Find( id );
        }

        public Student Update( Student studentChanges )
        {
            var student = _context.Students.Attach( studentChanges );
            student.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return studentChanges;
        }
    }
}
