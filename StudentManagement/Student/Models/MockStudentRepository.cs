﻿using System.Collections.Generic;
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
                new Student(){Id = 1, Name = "Oguns", Department = Dept.ComputerSci, Email = "oguns@student.com"},
                new Student(){Id = 2, Name = "Kiki", Department = Dept.Act, Email = "kiki@student.com"},
                new Student(){Id = 1, Name = "Ruka", Department = Dept.English, Email = "ruka@student.com"}
            };
        }

        public Student Add( Student student )
        {
            student.Id = _studentList.Max( s => s.Id ) + 1;
            _studentList.Add( student );
            return student;
        }

        public Student Delete( int Id )
        {
            Student student = _studentList.FirstOrDefault( s => s.Id == Id );
            if(student != null)
            {
                _studentList.Remove( student );
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudent( )
        {
            return _studentList;
        }

        public Student GetStudent( int id )
        {
            return _studentList.FirstOrDefault( e => e.Id == id );
        }

        public Student Update( Student studentChanges )
        {
            Student student = _studentList.FirstOrDefault( s => s.Id == studentChanges.Id );
            if(student != null)
            {
                student.Name = studentChanges.Name;
                student.Email = studentChanges.Email;
                student.Department = studentChanges.Department;
            }
            return student;
        }
    }
}
