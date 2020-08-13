using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student.Models;
using Student.ViewModels;
using System;
using System.IO;

namespace Student.Controllers
{
    public class HomeController : Controller
    {
        //Constructor Injection
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController( IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment )
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public ViewResult Index( )
        {
            var model = _studentRepository.GetAllStudent();
            return View( model );
        }

        public ViewResult Details( int? Id )
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Student = _studentRepository.GetStudent( Id ?? 1 ),
                PageTitle = "Student Details"
            };

            return View( homeDetailsViewModel );
        }

        [HttpGet]
        public ViewResult Create( )
        {
            return View();
        }

        [HttpGet]
        public ViewResult Edit( int Id )
        {
            var student = _studentRepository.GetStudent( Id );
            var studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Department = student.Department,
                ExistingPhotoPath = student.PhotoPath
            };
            return View( studentEditViewModel );
        }

        [HttpPost]
        public IActionResult Create( StudentCreateViewModel model )
        {
            if(ModelState.IsValid)
            {
                string uniqueFileName = null;
                if(model.Photos != null && model.Photos.Count > 0)
                {
                    foreach(IFormFile photo in model.Photos)
                    {
                        var uploadsFolder = Path.Combine( _webHostEnvironment.WebRootPath, "images" );
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        var filePath = Path.Combine( uploadsFolder, uniqueFileName );
                        photo.CopyTo( new FileStream( filePath, FileMode.Create ) );
                    }
                }

                var newStudent = new Models.Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _studentRepository.Add( newStudent );
                return RedirectToAction( "Details", new { Id = newStudent.Id } );
            }

            return View();

        }
    }
}
