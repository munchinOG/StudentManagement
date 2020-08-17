using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Student.Models;
using Student.ViewModels;
using System;
using System.IO;

namespace Student.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //Constructor Injection
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger _logger;

        public HomeController( IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger )
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [AllowAnonymous]
        public ViewResult Index( )
        {
            var model = _studentRepository.GetAllStudent();
            return View( model );
        }

        [AllowAnonymous]
        public ViewResult Details( int? id )
        {
            //throw new Exception( "Error in Details View" );

            _logger.LogTrace( "Trace Log" );
            _logger.LogDebug( "Debug Log" );
            _logger.LogInformation( "Information Log" );
            _logger.LogWarning( "Warning Log" );
            _logger.LogError( "Error Log" );
            _logger.LogCritical( "Critical Log" );

            var student = _studentRepository.GetStudent( id.Value );

            if(student == null)
            {
                Response.StatusCode = 404;
                return View( "StudentNotFound", id.Value );
            }
            var homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Student = student,
                PageTitle = "Student Details"
            };

            return View( homeDetailsViewModel );
        }

        [HttpGet]
        [Authorize]
        public ViewResult Create( )
        {
            return View();
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
