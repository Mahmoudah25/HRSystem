using HRManagmentSystem.DTOs.PayRoll;
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
    public class PayrollController : ControllerBase
    {
        private readonly HRContext context;
        public PayrollController(HRContext context)
        {
            this.context = context;
        }

        //GetAllPayRoll
        [HttpGet("GetAll")]
        public IActionResult GetAllPayRoll() 
        {
            var res =  context.Payrolls.ToList();
            if(!res.Any()) 
                return NotFound();
            return Ok(res);
        }

        //GetRollById
        [HttpGet("GetRollById/{id:int}")]
        public IActionResult GetbyId(int id) 
        {
            var res = context.Payrolls.FirstOrDefault(x => x.Id == id);
            if(res == null)
                return NotFound(" PayRoll Not Found ");
            return Ok(res);
        }

        //GetByEmployee
        [HttpGet("GetByEmployee/{EmployeeId:int}")]
        public IActionResult GetByEmployeeId(int EmployeeId) 
        {
            var foound = context.Payrolls.Where(x => x.EmployeeId == EmployeeId).ToList();
            if (!foound.Any())
                return NotFound(" No PayRolls For This employee");
            return Ok(foound);
        }

        //GetByMonth 7 Year
        [HttpGet("GetByDate/{Month:int}/{Year:int}")]
        public IActionResult GetByDate(int Month, int Year) 
        {
            var res = context.Payrolls.Where( x=>x.Month == Month && x.Year == Year).ToList();
            if (!res.Any())
                return NotFound(" No PayRolls For This Date");
            return Ok(res);
        }

        //AddPayRolls
        [HttpPost("AddPayRoll")]
        public IActionResult AddRolls(AddPayRollDTO model)
        {
             if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Exist = context.Payrolls.Any(x => x.EmployeeId == model.EmployeeId &&
                 x.Month == model.Month && x.Year == model.Year);
            if (Exist == null)
                return BadRequest(" PayRoll Already Exist");
            var ExistEmp = context.Employees.Any(x => x.Id == model.EmployeeId);
            if(!ExistEmp)
                return BadRequest(" Employee Not Found");
            var payroll = new Payrolls
            {
                Month = model.Month,
                Year = model.Year,
                BaseSalary = model.BaseSalary,
                Allowances = model.Allowances,
                Deductions = model.Deductions,
                NetSalary = (model.BaseSalary + model.Allowances) - model.Deductions,
                EmployeeId = model.EmployeeId
            };
            context.Payrolls.Add(payroll);
            context.SaveChanges();
            return Ok(payroll);
        }

        //EditpayRolls
        [HttpPut("EditRolls/{id:int}")]
        public IActionResult EditPayRolls(int id, EditPayRollsDTO model) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var found = context.Payrolls.Find(id);
            if(found != null)
            {
                found.Month = model.Month;
                found.Year = model.Year;
                found.BaseSalary = model.BaseSalary;
                found.Allowances = model.Allowances;
                found.Deductions = model.Deductions;
                found.NetSalary = (model.BaseSalary + model.Allowances) - model.Deductions;
                context.SaveChanges();
                return Ok(found);
            }
            return NotFound(" PayRoll Not Found");
        }

        //DeletePayroll
        [HttpDelete("DeletePayRoll/{id:int}")]
        public IActionResult DeletePayRolls(int id) 
        {
            var found = context.Payrolls.Find(id);
            if (found != null) 
            {
                context.Payrolls.Remove(found);
                context.SaveChanges();
                return Ok(found);
            }
            return NotFound(" PayRoll Not Found");
        }

        //GetTotalForMonth
        [HttpGet("GetTotalForMonth")]
        public IActionResult GetTotalForMonth(int month , int year)
        {
            var res = context.Payrolls.Where(x => x.Month == month && x.Year == year)
                .Sum(c => c.NetSalary);
            return Ok(new
            {
                TotalNetSalary = res
            });
        }

        //CalcuteNetSalary
        [HttpPost("Calcuate")]
        public IActionResult CalcuatePayRoll([FromBody] CalculatePayrollDTO model)
        {
            var ExistEmp = context.Employees.FirstOrDefault(x => x.Id == model.EmployeeId);
            if (ExistEmp == null)
                return NotFound(" Employee Not Found");
            decimal netSalary = (model.BaseSalary + model.Allowances) - model.Deductions;
            var payroll = new Payrolls
            {
                EmployeeId = model.EmployeeId,
                Month = model.Month,
                Year = model.Year,
                BaseSalary = model.BaseSalary,
                Allowances = model.Allowances,
                Deductions = model.Deductions,
                NetSalary = netSalary
            };
            context.Payrolls.Add(payroll);
            context.SaveChanges();
            return Ok(new
            {
                Message =" PayRoll calculated Successfully and Saved "
                ,payRollId = payroll.Id,
                NetSalary = netSalary
            });
        }

        //GetLatest
        [HttpGet("Latest/{count:int}")]
        public IActionResult GetLatest(int count)
        {
            var data = context.Payrolls
                .OrderByDescending(o => o.Id)
                .Take(count)
                .ToList();
            return Ok(data);
        }

        

    }
}
