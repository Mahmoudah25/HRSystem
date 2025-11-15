using HRManagmentSystem.DTOs.Traning;
using HRManagmentSystem.Models;
using HRManagmentSystem.Models.Traning_Performance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;

namespace HRManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TraningController : ControllerBase
    {
        private readonly HRContext context;
        public TraningController(HRContext context) 
        {
            this.context = context;
        }

        //GetAllTraing
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetAll()
        {
            var res = context.Trainings.ToList();
            return Ok(res);
        }

        //GetAllTraingByID
        [HttpGet("GetBy/{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetById(int id) 
        {
            var res = context.Trainings.FirstOrDefault(c => c.Id == id);
            if (res == null) 
                return NotFound();
            return Ok(res);
        }

        //CreateTraing
        [Authorize(Roles = "Admin,HR")]
        [HttpPost("Add")]
        public IActionResult Add(AddTraningDTO model)
        {
            var training = new Traning
            {
                Title = model.Title,
                Startdate = model.Startdate,
                Enddate = model.Enddate
            };
            context.Trainings.Add(training);
            context.SaveChanges();  
            return Ok(training);
        }

        //Edit
        [Authorize(Roles = "Admin,HR")]
        [HttpPut("Update{id:int}")]
        public IActionResult Edit(int id,UpdateTraingDTO model)
        {
            var found = context.Trainings.Find(id);
            if(found == null)   
                return NotFound();
            found.Title = model.Title;
            found.Startdate = model.Startdate;
            found.Enddate = model.Enddate;
            context.SaveChanges();
            return Ok(found);
        }

        //Delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete{id:int}")]
        public IActionResult Delete(int id)
        {
            var found = context.Trainings.Find(id);
            if (found == null)
                return NotFound();
            context.Trainings.Remove(found);
            context.SaveChanges();
            return Ok();
        }

        //Add Employee to sprcial trainig
        [Authorize(Roles = "Admin ,HR")]
        [HttpPost("{TrainningID:int}/AssignTo/{EmployeeId:int}")]
        public IActionResult AssignEmployee(int TrainningId , int EmployeeId)
        {
            var FoundTran = context.Trainings.Find(TrainningId);
            var FoundEmp = context.Employees.Find(EmployeeId);
            if (FoundTran == null || FoundEmp == null)
                return NotFound("Traing Or Employee Not Found");
            var Exist = context.EmployeeTrainings.Any(x => x.EmployeeId == EmployeeId);
            if (Exist)
                return BadRequest(" Employee Already Assign");
            var empTraing = new EmployeeTraning
            {
                TraningId = TrainningId,
                EmployeeId = EmployeeId
            };
            context.EmployeeTrainings.Add(empTraing);
            context.SaveChanges();
            return Ok("Employee Assign To Tranning");
        }

        //delete Employee from sprcial trainig
        [Authorize(Roles = "Admin ,HR")]
        [HttpDelete("{TrainningID:int}/RemoveEmployee/{EmployeeId:int}")]
        public IActionResult DeleteEmployee(int TrainningId, int EmployeeId)
        {
            var found = context.EmployeeTrainings.FirstOrDefault
                (i => i.TraningId == TrainningId && i.EmployeeId == EmployeeId);
            if (found == null)
                return NotFound("Employee Not Assign To Training");
            context.EmployeeTrainings.Remove(found);
            return Ok(" Employee Removed From Trainning");
        }

        [HttpGet("{trainingId}/Employees")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetEmployeesInTraining(int trainingId)
        {
            var employees = context.EmployeeTrainings
                .Where(e => e.TraningId == trainingId)
                .Select(e => new
                {
                    e.Employee.Id,
                    e.Employee.FullName
                }).ToList();

            return Ok(employees);
        }

        //Dispaly Traing For Spectfic Employee
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("Employee/{employeeId:int}")]
        public IActionResult GetTrainingForEmployee(int employeeId)
        {
            var traings = context.EmployeeTrainings.Where(x => x.EmployeeId == employeeId)
                .Select(e => new
                {
                    e.traning.Id,
                    e.traning.Title,
                    e.traning.Startdate,
                    e.traning.Enddate
                }).ToList();
            return Ok(traings);
        }

        //Search
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("Search/{title:alpha}")]
        public IActionResult Search( string title)
        {
            var traing = context.Trainings.Where(t => t.Title.Contains(title)).ToList();
            return Ok(traing);
        }

        //upComing Traings
        [HttpGet("UpComing")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult UpComing()
        {
            var found = context.Trainings.Where(t => t.Startdate > DateTime.Now)
                .ToList();
            return Ok(found);
        }

        //Finished Traings
        [HttpGet("Finished")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Finished()
        {
            var found = context.Trainings.Where(p => p.Enddate < DateTime.Now)
                .ToList();
            return Ok(found);

        }

        //Update Status
        [Authorize(Roles = "Admin,HR")]
        [HttpPut("{id:int}/UpdateStatus")]
        public IActionResult UpdateStatus(int id, [FromBody] TrainigStatus status)
        {
            var trainig = context.Trainings.Find(id);
            if (trainig == null)
                return NotFound("Training Not Found");
            trainig.Status = status;
            context.SaveChanges();
            return Ok(new
            {
                message = " Status Updated Successfully"
                ,Status = status
            });
        }

        //Addcertificate
        [HttpPost("{trainingId:int}/AddCertificate/{EmployeeId:int}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult AddCertificate(int trainingId, int EmployeeId, [FromBody] string CertificateName)
        {
            var trainig = context.Trainings.Find(trainingId);
            var employee = context.Employees.Find(EmployeeId);
            if (trainig == null || employee == null)
                return NotFound(" Traing Or Employee Not Found");
            var cert = new TrainingCertificate
            {
                TrainingId = trainingId,
                EmployeeId = EmployeeId,
                CertificateName = CertificateName,
                IssuedAt = DateTime.Now,
            };
            context.trainingCertificates.Add(cert);
            context.SaveChanges();
            return Ok( new
            {
                Message =" Certificate Created Successful",
                Certificate = cert
            });
        }

        //Display Certificate for Special Employee
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("Employee/{employeeId:int}/Certificate")]
        public IActionResult DisplayCertificateForSpecialEmployee(int employeeId)
        {
            var certs = context.trainingCertificates.Where(c => c.EmployeeId == employeeId
            ).ToList();
            if (!certs.Any())
                return NotFound(" No Certificate Found");
            return Ok(certs);
        }

        //Display Certificate for Special Traning
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("{TrainingId:int}/Certificate")]
        public IActionResult DisplayCertificateForSpecialTraning(int TrainingId)
        {
            var found = context.trainingCertificates.Where(c => c.TrainingId == TrainingId)
                .ToList();
            if (!found.Any())
                return NotFound(" No Certificate Found");
            return Ok(found);
        }

    }
}
