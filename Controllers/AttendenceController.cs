using HRManagmentSystem.DTOs.Attendece;
using HRManagmentSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AttendenceController : ControllerBase
    {
        private readonly HRContext context;
        public AttendenceController(HRContext context)
        {
            this.context = context;
        }

        //Check IN
        [Authorize(Roles = "Employee")]
        [HttpPost("CheckIn")]
        public IActionResult CheckIn(int employeeId)
        {
            var todayattendece = context.Attendances.FirstOrDefault
                ( x=>x.EmployeeId  == employeeId && x.Date == DateTime.Today);
            if (todayattendece != null)
                return BadRequest(" Employee Already Ckeck In Today");
            var attendece = new Attendance
            {
                EmployeeId = employeeId,
                Date = DateTime.Today,
                CheckIn = DateTime.Now,
                Status = "Present"
            };
            context.Attendances.Add(attendece);
            context.SaveChanges();
            return Ok(attendece);
        }

        //Check Out
        [HttpPut("CheckOut")]
        [Authorize(Roles = "Employee")]
        public IActionResult CheckOut(int id)
        {
            var found = context.Attendances.Find(id);
            if (found ==  null)
                return NotFound(" Attendece Recored Not Found");
            found.CheckOut = DateTime.Now;
            found.HoursWorked = (found.CheckOut - found.CheckIn)?.TotalHours;
            context.SaveChanges();
            return Ok(found);
        }

        //Get All Attendece
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetEmployee/{empid:int}")]
        public IActionResult GetEmployee(int empid) 
        {
            var records = context.Attendances.Include(a => a.Employee).Where(x=>x.EmployeeId == empid).ToList();
            return Ok(records);
        }

        //Get By date
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetByDate")]
        public IActionResult GetByDate(DateTime date)
        {
            var found = context.Attendances.Where(c=>c.Date == date).ToList();
            return Ok(found);
        }

        //delete Attendece 
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteAttendece")]
        public IActionResult DeleteAttendece(int id)
        {
            var found = context.Attendances.Find(id);
            if (found == null)
                return NotFound();
            context.Attendances.Remove(found);
            context.SaveChanges();
            return Ok(found);
        }

        //reports
        [HttpGet("Reports")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Reports(DateTime StartDate, DateTime Enddate ,int? employeeId = null) 
        {
            var qeury = context.Attendances.Include(a => a.Employee)
                .Where(a => a.Date.Date >= StartDate && a.Date.Date <= Enddate);
            if(employeeId.HasValue)
                qeury = qeury.Where(a=>a.EmployeeId == employeeId.Value);
            var report = qeury
                .Select(a => new
                {
                    a.EmployeeId,
                    a.Date,
                    a.CheckIn,
                    a.CheckOut,
                    a.Status
                })
                .OrderBy(a=>a.Date)
                .ToList();
            return Ok(report);
        }

        //filter
        [HttpGet("Filter")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Filter(int employeeId ,DateTime strattime,DateTime endtime)
        {
            var records = context.Attendances.Where(a=>
            a.EmployeeId == employeeId && 
            a.Date.Date >= strattime &&
            a.Date.Date  <=endtime
            ).OrderBy(a=>a.Date)
            .ToList();
            return Ok(records);
        }

        //GetAllAttendece
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Getall()
        {
            var res = context.Attendances.ToList();
            return Ok(res);
        }

        //GetToday
        [HttpGet("ToDay")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetToday()
        {
            var today = DateTime.Today;
            var res = context.Attendances.Where(x => x.Date.Date == today)
                .ToList();
            return Ok(res);
        }

        //GetLateEmploee
        [HttpGet("LatestEmployees")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetLateEmploee(TimeSpan workStart)
        {
            var late = context.Attendances.Where(a => a.CheckIn.HasValue
            && a.CheckIn.Value.TimeOfDay > workStart)
                .ToList();
            return Ok(late);
        }

        //updateAttendece
        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "HR")]
        public IActionResult updateAttendece(int id , [FromBody] UpdateAttendeceDTO model)
        {
            var found = context.Attendances.Find(id);
            if (found == null)
                return NotFound("Attendance not found");
            if (model.CheckIn.HasValue)
                found.CheckIn = model.CheckIn.Value;
            if (model.CheckOut.HasValue)
                found.CheckOut = model.CheckOut.Value;
            if (!string.IsNullOrEmpty(model.Status))
                found.Status = model.Status;
            if (found.CheckIn != null && found.CheckOut != null)
                found.HoursWorked = (found.CheckOut - found.CheckIn)?.TotalHours;
            context.SaveChanges();
            return Ok(found);

        }
    }
}
