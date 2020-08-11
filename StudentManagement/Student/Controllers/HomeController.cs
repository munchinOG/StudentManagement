﻿using Microsoft.AspNetCore.Mvc;
using Student.Models;
using Student.ViewModels;

namespace Student.Controllers
{
    public class HomeController : Controller
    {
        //Constructor Injection
        private readonly IStudentRepository _studentRepository;
        public HomeController( IStudentRepository studentRepository )
        {
            _studentRepository = studentRepository;
        }

        public ViewResult Index( )
        {
            var model = _studentRepository.GetAllStudents();
            return View( model );
        }

        public ViewResult Details( int Id )
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Student = _studentRepository.GetStudent( Id ),
                PageTitle = "Student Details"
            };

            return View( homeDetailsViewModel );
        }
    }
}
