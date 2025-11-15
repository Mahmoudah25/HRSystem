using HRManagmentSystem.DTOs.OverTime;
using HRManagmentSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OverTimeController : ControllerBase
    {
        private readonly HRContext context;
        public OverTimeController(HRContext context)
        {
            this.context = context;
        }

        //GetAll
        [Authorize(Roles ="Admin,HR")]
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var res = context.Overtimes.ToList();
            if (!res.Any())
                return NotFound(" No OverTime Found");
            return Ok(res);
        }

        //GetById
        [HttpGet("GetById/{id:int}")]
        public IActionResult GetById(int id)
        {
            var found = context.Overtimes.FirstOrDefault(x => x.Id == id);
            if (found == null)
                return NotFound(" OverTime Not Found");
            return Ok(found);
        }

        //Add OverTime
        [HttpPost("Add")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Add([FromBody] AddOverTimeDTO model) 
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!context.Employees.Any(e => e.Id == model.EmployeeId))
                return NotFound(" Employee Not Found");
            var over = new OverTime
            {
                Date = model.Date,
                hours= model.hours,
                Rate = model.Rate,
                EmployeeId = model.EmployeeId
            };
            context.Overtimes.Add(over);
            context.SaveChanges();
            return Ok(over);
        }

        //Edit
        [HttpPut("Edit/{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Edit(int id , [FromBody] EditOverTimeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var found = context.Overtimes.Find(id);
            if (found == null) return NotFound("OverTime Not Found");
            found.Date = model.Date;
            found.hours = model.hours;
            found.Rate = model.Rate;
            context.SaveChanges();
            return Ok(found);
        }

        //Delete
        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Delete(int id) 
        {
            var found = context.Overtimes.Find(id);
            if (found != null) 
            {
                context.Overtimes.Remove(found);
                context.SaveChanges();
                return Ok(found);
            }
            return NotFound("OverTime Not Found");
        }

        //Get OverTime For Spectfic Employee
        [HttpGet("GetByEmployee/{empId:int}")]
        public IActionResult GetOverTimeForSpectficEmployee(int empId)
        {
            var res = context.Overtimes.Where(o => o.EmployeeId == empId)
                .ToList();
            if (!res.Any())
                return NotFound(" No Over Time For This Employee");
            return Ok(res);
        }

        //Get Amount
        [HttpGet("TotalAmount/{empId:int}")]
        public IActionResult GetAmount(int empId)
        {
            var total = context.Overtimes.Where(x => x.EmployeeId == empId)
                .Sum(o => o.Rate * (decimal)o.hours);
            return Ok(new
            {
                EmployeeId=empId,
                TotalOverTimeAmount = total
            });
        }

        //GetOverTimeByDateRange
        [HttpGet("GetByDateRange")]
        public IActionResult GetOverTimeByDateRange(DateTime StratTime ,DateTime EndTime)
        {
            var res = context.Overtimes.Where(o => o.Date >= StratTime && o.Date <= EndTime)
                .ToList();
            if (!res.Any())
                return NotFound(" No OverTime In This Range");
            return Ok(res);
        }

        //GetTotalHoursForEmployee
        [HttpGet("Totalhours{empId:int}")]
        public IActionResult GetTotalHoursForEmployee(int empId)
        {
            var TotalHours = context.Overtimes.Where(x => x.EmployeeId == empId)
                .Sum(p => p.hours);
            return Ok( new
            {
                EmployeeID = empId,
                TotalHours = TotalHours
            });
        }

        //GetMonthlyOverTime
        [HttpGet("GetMonthly")]
        public IActionResult GetMonthlyOverTime(int month , int year)
        {
            var found = context.Overtimes.Where
                (o => o.Date.Month == month && o.Date.Year == year)
                .ToList();
            if (!found.Any())
                return NotFound(" No OverTimes At This Month");
            return Ok(found);
        }

        //GetTopEmployeesOvertime
        [HttpGet("TopOverTime")]
        public IActionResult GetTopEmployeesOvertime(int count = 5)
        {
            var res = context.Overtimes
                .GroupBy(o => o.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    TotalHours = g.Sum(o => o.hours),
                    TotalAmount = g.Sum(o => o.hours * (double)o.Rate)
                })
                .OrderByDescending(X => X.TotalHours)
                .Take(count)
                .ToList();
            return Ok( res );   
        }
    }
}
