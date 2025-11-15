using HRManagmentSystem.DTOs.Employee;
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
    public class EmployeeController : ControllerBase
    {
        private readonly HRContext context;
        public EmployeeController(HRContext context)
        {
            this.context = context;
        }

        //GetAllEmployee
        [HttpGet("GetAllEmployee")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult GetAllEmployee()
        {
            var result = context.Employees.ToList();
            return Ok(result);
        }

        //GetById
        [HttpGet("GetById{id:int}")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult GetById(int id)
        {
            var result = context.Employees.FirstOrDefault(x => x.Id == id);
            if (result == null)
                return NotFound("Employee Not Found");
            return Ok(result);
        }

        //GetByName
        [HttpGet("GetByName{name:alpha}")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult GetByName(string name)
        {
            var result = context.Employees.FirstOrDefault(x => x.FullName == name);
            if (result == null)
                return NotFound("Employee Not Found");
            return Ok(result);
        }

        //AddEmployee
        //[HttpPost("AddEmployee")]
        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("AddEmployee")]
        public IActionResult AddEmployee([FromBody] AddEmployeeDTO model)
        {
            try
            {
                var employee = new Employee
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Salary = model.Salary,
                    HireDate = model.HireDate,
                    DepartmentId = model.DepartmentId,
                    PositionId = model.PositionId
                };

                context.Employees.Add(employee);
                context.SaveChanges();
                return Ok($"Employee {model.FullName} Added Successfully ");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
        //BulkInsert
        [HttpPost("BulkInsert")]
        public IActionResult BulkInsert(List<AddEmployeeDTO> models)
        {
            if (models == null || !models.Any())
                return BadRequest(" Must be Add One Employee At Least");
            var newEmployess = new List<Employee>();
            foreach (var model in models)
            {
                var emp = new Employee
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Salary = model.Salary,
                    HireDate = model.HireDate,
                    DepartmentId = model.DepartmentId,
                    PositionId= model.PositionId
                };
                newEmployess.Add(emp);
            }
            context.Employees.AddRange(newEmployess);
            context.SaveChanges();
            return Ok(new
            {
                Message = $"{newEmployess.Count} Employees Added Successfully",
                Employees = newEmployess
            });
        }

        //DeleteEmployee
        [HttpDelete("{id:int}/DeleteEmployee")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteEmployee(int id)
        {
            var found = context.Employees.Find(id);
            if (found == null)
                return NotFound("Employee Not Found");
            //context.Employees.Remove(found);
            found.IsDeleted = true;
            context.SaveChanges();
            return Ok($"Employee{found.FullName} Deleted Successfully");
        }

        //Restore Employee
        [Authorize(Roles = "Admin,Customer")]
        [HttpPut("{id:int}/Restore")]
        public IActionResult RestoreEmployee(int id)
        {
            var emp = context.Employees.FirstOrDefault(x => x.Id == id && x.IsDeleted == true);
            if (emp == null)
                return BadRequest(" Empoyee Not Found");
            emp.IsDeleted = false;
            context.SaveChanges();
            return Ok(new
            {
                Message = $" Employee {emp.FullName} restored Successfully"
            });
        }

        //EditEmployee
        [Authorize(Roles = "Admin,Customer")]
        [HttpPut("{id:int}/EditEmployee")]
        public IActionResult EditEmployee(int id, EditEmployeeDTO model)
        {
            var found = context.Employees.Find(id);
            if (found == null)
                return NotFound("Employee Not Found");
            found.FullName = model.FullName;
            found.Email = model.Email;
            found.Salary = model.Salary;
            found.HireDate = model.HireDate;
            found.DepartmentId = model.DepartmentId;
            found.PositionId = model.PositionId;
            context.SaveChanges();
            return Ok($"Employee  {model.FullName}  Updates Successfully");
        }

        //Search
        [HttpGet("Search")]
        [Authorize(Roles = "Admin")]
        public IActionResult Search(string? name, string? email)
        {
            var qeury = context.Employees.AsQueryable();
            if (!string.IsNullOrEmpty(name))
                qeury = qeury.Where(e => e.FullName == name);
            if (!string.IsNullOrEmpty(email))
                qeury = qeury.Where(e => e.Email == email);
            return Ok(qeury.ToList());
        }

        //GetEmployees By Department
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("ByDepartment{id:int}")]
        public IActionResult GetEmpsByDept(int id)
        {
            var res = context.Employees
                .Where(con => con.DepartmentId == id && !con.IsDeleted)
                .ToList();
            return Ok(res);
        }

        //SalaryStats
        [HttpGet("SalaryStats")]
        public IActionResult SalaryStats()
        {
            var total = context.Employees.Sum(e => e.Salary);
            var avg = context.Employees.Average(s => s.Salary);
            var count = context.Employees.Count();
            return Ok(new
            {
                TotalSalary = total,
                AverageSalary = avg,
                Count = count
            });
        }

        //Sort
        [HttpGet("SortBy")]
        public IActionResult SortBy(string field = "name", string order = "asc")
        {
            var employees = context.Employees.AsQueryable();

            employees = (field.ToLower(), order.ToLower()) switch
            {
                ("name", "asc") => employees.OrderBy(e => e.FullName),
                ("name", "desc") => employees.OrderByDescending(e => e.FullName),
                ("salary", "asc") => employees.OrderBy(e => e.Salary),
                ("salary", "desc") => employees.OrderByDescending(e => e.Salary),
                _ => employees
            };

            return Ok(employees.ToList());
        }

        //GetActiveEmployees
        [HttpGet("Active")]
        public IActionResult GetActiveEmployees()
        {
            var emps = context.Employees.Where(e => !e.IsDeleted).ToList();
            return Ok(emps);
        }

        //GetDeletedEmployees
        [HttpGet("Deleted")]
        public IActionResult GetDeletedEmployees()
        {
            var emps = context.Employees.Where(e => e.IsDeleted).ToList();
            return Ok(emps);
        }

        //Paged
        [HttpGet("Paged")]
        public IActionResult GetPaged(int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;
            var data = context.Employees
                .Where(e => !e.IsDeleted)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                Data = data
            });
        }

        //ChangeEmployeeStatus
        [HttpPut("{id:int}/ToggleStatus")]
        public IActionResult ChangeEmployeeStatus(int id)
        {
            var emp = context.Employees.Find(id);
            if (emp == null)
                return NotFound("Employee Not Found");
            emp.IsDeleted = !emp.IsDeleted;
            context.SaveChanges();
            return Ok(new
            {
                Message = emp.IsDeleted ? 
                "Employee Disabled" : "Employee Activate"
                ,emp.Id,
                emp.FullName
            } 
                );
        }

        //GetEmployeeProfile
        [HttpGet("Profile/{id:int}")]
        public IActionResult GetEmployeeProfile(int id)
        {
            var emp = context.Employees.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (emp == null)
                return NotFound(" Employee Not Found");
            return Ok(emp);
        }

        //Count
        [HttpGet("Count")]
        public IActionResult Count()
        {
            var empscount = context.Employees.Count(e => !e.IsDeleted);
            return Ok(new {Count = empscount });
        }



    }
}
