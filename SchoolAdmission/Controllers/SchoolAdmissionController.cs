using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolAdmission.BusinessLayer.Interfaces;
using SchoolAdmission.BusinessLayer.Services;
using SchoolAdmission.BusinessLayer.ViewModels;
using SchoolAdmission.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolAdmission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolAdmissionController : ControllerBase
    {
        private readonly IStudentServices _studentService;
        private readonly IClassroomServices _classroomService;
        private readonly IAdmissionServices _admissionService;

        public SchoolAdmissionController(IStudentServices studentServices, IClassroomServices classroomServices, IAdmissionServices admissionServices)
        {
            _studentService = studentServices;
            _classroomService = classroomServices;
            _admissionService = admissionServices;
        }

        
        [HttpPost]
        [Route("Student/Register-Student")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] StudentViewModel model)
        {
            var studentExists = await _studentService.FindPersonById(model.PersonID);
            if (studentExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Person already exists!" });
            Person person = new Person()
            {
                PersonID = model.PersonID,
                PersonName = model.PersonName,
                EmailId = model.EmailId,
                Password = model.Password,
                Personcity = model.Personcity
                
            };
            var result = await _studentService.Register(person);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Person creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "Person created successfully!" });

        }

        
        [HttpPost]
        [Route("Student/Login-Student")]
        public async Task<IActionResult> Login(string emailid,string password)
        {
            var result = await _studentService.Login(emailid,password);
            if (result == false)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = $"Student With emailId = {emailid} cannot be found" });
            }
            else
            {
                return Ok(new Response { Status = "Success", Message = "Logged In successfully!" });
            }
        }

        [HttpPost]
        [Route("Classroom/Add-Classroom")]
        [AllowAnonymous]
        public async Task<IActionResult> AddClassroom([FromBody] ClassroomViewModel model)
        {
            var classroomExists = await _classroomService.FindClassroomById(model.ClassroomId);
            if (classroomExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Classroom already exists!" });
            Classroom classroom = new Classroom()
            {
                ClassroomId = model.ClassroomId,
                ClassType = model.ClassType,
                PerClassFee = model.PerClassFee,
                StudentCapacity = model.StudentCapacity,
                Availability = model.Availability

            };
            var result = await _classroomService.AddClassroom(classroom);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Classroom creation failed! Please check details and try again." });

            return Ok(new Response { Status = "Success", Message = "Classroom created successfully!" });

        }


        /// <summary>
        /// List All Suppliers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Classroom/ListAvailable-Classrooms")]
        public async Task<IEnumerable<Classroom>> ListAvailableClassrooms()
        {
            return await _classroomService.GetClassroom();
        }


        [HttpPost]
        [Route("Admission/Add-Admission")]
        [AllowAnonymous]
        public async Task<IActionResult> AddAdmission([FromBody] AdmissionViewModel model)
        {
            var admissionExists = await _admissionService.FindAdmissionById(model.AdmissionId);
            if (admissionExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Admission details already exists!" });
            Admission admission = new Admission()
            {
                AdmissionId = model.ClassroomId,
                ClassroomId = model.ClassroomId,
                PersonId = model.PersonId,
                NumberofStudents = model.NumberofStudents,
                
            };
            var result = await _admissionService.AddAdmission(admission);
            if (result == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Admission creation failed! Please check details and try again." });

            return Ok(new Response { Status = "Success", Message = "Admission created successfully!" });

        }


        /// <summary>
        /// List All Suppliers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Admission/ListAll-Admissions")]
        public async Task<IEnumerable<Admission>> ListAllAdmissions()
        {
            return await _admissionService.GetAdmission();
        }


    }
}