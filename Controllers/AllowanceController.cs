using HRManagmentSystem.DTOs.Allowance;
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
    public class AllowanceController : ControllerBase
    {
        private readonly HRContext context;
        public AllowanceController(HRContext context)
        {
            this.context = context;
        }

        //GetAll Allowane
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetAll")]
        public IActionResult GetAllAllownce()
        {
            var allow = context.Allowances.Include(a => a.Employee).ToList();
            if (!allow.Any())
                return NotFound();
            return Ok(allow);
        }

        //GetById
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetById/{id:int}")]
        public IActionResult getById(int id)
        {
            var allow = context.Allowances.Include(a => a.Employee)
                .FirstOrDefault(a => a.Id == id);
            if (allow == null)
                return NotFound();
            return Ok(allow);
        }

        //GetByemployee
        [HttpGet("GetByEmployee/{emolyeeid:int}")]
        [Authorize(Roles = "Admin,HR,Employee")]
        public IActionResult GetByEmployee( int emolyeeid)
        {
            var allow = context.Allowances.Where(x => x.EmployeeId == emolyeeid)
                .Include(p => p.Employee).ToList();
            if (!allow.Any())
                return NotFound("No Allowance For This Employee");
            return Ok(allow);
        }

        //Get By Type
        [HttpGet("GetByType/{type:alpha}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult GetByType(string type)
        {
            var res = context.Allowances.Where(x => x.Type == type)
                .Include(i=>i.Employee)
                .ToList();
            if(res == null)
                return NotFound();
            return Ok(res);
        }
        //AddAllowance
        [HttpPost("AddAllowance")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult AddAllowance([FromBody] AddAllowanceDTO model) 
        {
            var Exist = context.Employees.Any(e=>e.Id == model.EmployeeId);
            if(!Exist) 
                return NotFound();
            var allowance = new Allowwance
            {
                Type = model.Type,
                Amount = model.Amount,
                EmployeeId= model.EmployeeId
            };
            context.Allowances.Add(allowance);
            context.SaveChanges();
            return CreatedAtAction(nameof(getById), new { id=allowance.Id } ,allowance);
        }

        //EditAllowance
        [HttpPut("EditAllowance/{id:int}")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult EditAllowance(int id, [FromBody] EditAllowanceDTO model) 
        {
            var exist = context.Allowances.Find(id);
            if(exist == null)
                return NotFound();
            exist.Type = model.Type;
            exist.Amount = model.Amount;
            exist.EmployeeId = model.EmployeeId;
            context.SaveChanges();
            return Ok(exist);
        }

        //DeleteAllowance
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteAllowance{id:int}")]
        public IActionResult DeleteAllowance(int id) 
        {
            var exist = context.Allowances.Find(id);
            if (exist == null)
                return NotFound();
            context.Allowances.Remove(exist);
            context.SaveChanges();
            return Ok($" Allowance {exist.Type} Deleted Successfully ");
        }


        //Search
        [HttpGet("Search")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Search(string? type , decimal? minValue ,decimal? maxValue)
        {
            var qeury = context.Allowances.AsQueryable();
            if(!string.IsNullOrEmpty(type))
                qeury = qeury.Where(x=>x.Type.Contains(type));
            if(minValue.HasValue)
                qeury =qeury.Where(a=>a.Amount>= minValue.Value);   
            if(maxValue.HasValue)
                qeury =qeury.Where(a=>a.Amount<=maxValue.Value);
            return Ok(qeury.Include(a=>a.Employee).ToList());
        }
    }
}
