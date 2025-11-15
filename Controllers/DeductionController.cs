using HRManagmentSystem.DTOs.Deduction;
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
    public class DeductionController : ControllerBase
    {
        private readonly HRContext context;
        public DeductionController(HRContext context)
        {
            this.context = context;
        }

        //GetAllDeductions
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetAll")]
        public IActionResult GetAllDeductions()
        {
            var res = context.Deductions.ToList();
            if (!res.Any())
                return NotFound();
            return Ok(res);
        }

        //GetById
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetById/{id:int}")]
        public IActionResult GetById(int id)
        {
            var found = context.Deductions.FirstOrDefault(c => c.Id == id);
            if (found == null)
                return NotFound();
            return Ok(found);
        }

        //Getbyemployee
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetByemployee/{employeeId:int}")]
        public IActionResult GetByemployee(int employeeId)
        {
            var found = context.Deductions.FirstOrDefault(x=>x.EmployeeId == employeeId);
            if (found == null)
                return NotFound();
            return Ok(found);
        }

        //AddNewdeduction
        [HttpPost("Add")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Add([FromBody] AddDeductionDTO model) 
        {
            if (ModelState.IsValid) 
            {
                var Newded = new Deduction
                {
                    Reason = model.Reason,
                    Amount = model.Amount,
                    EmployeeId= model.EmployeeId
                };
                context.Deductions.Add(Newded);
                context.SaveChanges();
                return Ok(Newded);
            }
            return BadRequest(ModelState);
        }

        //Edit
        [HttpPut("Edit{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Edit(int id, EditDeductionDTO model)
        {
            if (ModelState.IsValid) 
            {
                var found = context.Deductions.Find(id);
                if(found == null)
                    return NotFound();
                found.Reason = model.Reason;
                found.Amount = model.Amount;
                context.SaveChanges();
                return Ok(found);
            }
            return BadRequest(ModelState);
        }

        //Delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id:int}")]
        public IActionResult Delete(int id) 
        {
            if (ModelState.IsValid)
            {
                var found = context.Deductions.Find(id);
                if (found == null)
                    return NotFound();
                context.Remove(found);
                context.SaveChanges();
                return Ok(found);
            }
            return BadRequest(ModelState);
        }

        //GetTodeduction For Employee
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("TotalByEmployee/{EmployeeId:int}")]
        public IActionResult GetTotaldeduction(int EmployeeId)
        {
            var total = context.Deductions.Where(x => x.EmployeeId == EmployeeId)
                .Sum(i => i.Amount);
            return Ok(new
            {
                EmployeeId =EmployeeId,
                Total = total
            });
        }

        //SearchByReason
        [HttpGet("Search")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Search([FromQuery]string Keyword)
        {
            var res = context.Deductions.Where(x => x.Reason.Contains(Keyword)).ToList();
            if (!res.Any())
                return NotFound();
            return Ok(res);
        }

        //Get Latest n Deduction
        [HttpGet("Latest/{count:int}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult getLatest(int count)
        {
            var res = context.Deductions
                .OrderByDescending(x => x.Id)
                .Take(count)
                .ToList();
            return Ok(res);
        }

        //group deduction By Employee
        [HttpGet("GroupByEmployee")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GroupByEmployee()
        {
            var result = context.Deductions
                .GroupBy(d => d.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    TotalDeductions = g.Sum(x => x.Amount),
                    Count = g.Count()
                });

            return Ok(result);
        }

        [HttpGet("Paged")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetPaged(int page = 1, int pageSize = 10)
        {
            var data = context.Deductions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(data);
        }

    }
    
}
