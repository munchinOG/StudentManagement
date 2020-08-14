using Microsoft.AspNetCore.Hosting;
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

        public ViewResult Details( int? id )
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Student = _studentRepository.GetStudent( id ?? 1 ),
                PageTitle = "Student Details"
            };

            return View( homeDetailsViewModel );
        }

        [HttpGet]
        public ViewResult Create( )
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create( StudentCreateViewModel model )
        {
            if(ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile( model );

                var newStudent = new Models.Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _studentRepository.Add( newStudent );
                return RedirectToAction( "Details", new { id = newStudent.Id } );
            }

            return View();

        }

        [HttpGet]
        public ViewResult Edit( int id )
        {
            var student = _studentRepository.GetStudent( id );
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
        public IActionResult Edit( StudentEditViewModel model )
        {
            if(ModelState.IsValid)
            {
                var student = _studentRepository.GetStudent( model.Id );
                student.Name = model.Name;
                student.Email = model.Email;
                student.Department = model.Department;
                if(model.Photo != null)
                {
                    if(model.ExistingPhotoPath != null)
                    {
                        var filePath = Path.Combine( _webHostEnvironment.WebRootPath,
                            "images", model.ExistingPhotoPath );
                        System.IO.File.Delete( filePath );
                    }

                    student.PhotoPath = ProcessUploadedFile( model );
                }

                var updatedStudent = _studentRepository.Update( student );
                return RedirectToAction( "Index" );
            }

            return View();
        }

        private string ProcessUploadedFile( StudentCreateViewModel model )
        {
            string uniqueFileName = null;
            if(model.Photo != null)
            {
                var uploadsFolder = Path.Combine( _webHostEnvironment.WebRootPath, "images" );
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                var filePath = Path.Combine( uploadsFolder, uniqueFileName );
                using(var fileStream = new FileStream( filePath, FileMode.Create ))
                {
                    model.Photo.CopyTo( fileStream );
                }
            }

            return uniqueFileName;
        }
    }
}
