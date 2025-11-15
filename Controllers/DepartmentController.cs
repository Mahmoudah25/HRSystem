using HRManagmentSystem.DTOs.Department;
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
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class DepartmentController : ControllerBase
    {
        private readonly HRContext context;
        public DepartmentController(HRContext context) 
        {
            this.context = context;
        }


        //GetAllDepaertment
        [HttpGet("GetAllDepartment")]
        [Authorize(Roles ="Admin,HR")]
        public IActionResult GetAllDepartment()
        {
            var result = context.Departments.Where(x => !x.IsDeleted).ToList();
            return Ok(result);
        }

        //GetByID
        [HttpGet("{id:int}/GetById")]
        [Authorize(Roles ="Admin,HR")]
        public IActionResult GetDeptById(int id) 
        {
            var res = context.Departments.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if(res == null)
                return NotFound(" Department Not Found");
            return Ok(res);
        }

        //GetByName
        [HttpGet("{name:alpha}/GetByName")]
        [Authorize(Roles ="Admin ,HR")]
        public IActionResult GetDeptByName(string name)
        {
            var res = context.Departments.FirstOrDefault(x => x.Name == name && !x.IsDeleted);
            if (res == null)
                return NotFound(" Department Not Found");
            return Ok(res);
        }

        //[HttpPost("AddDepartment")]
        ////[Authorize(Roles ="Admin ,HR")]
        //public IActionResult AddDepartment(AddDepartmentDTO model) 
        //{
        //    var department = new Department
        //    {
        //        Name = model.Name
        //    };
        //    context.Departments.Add(department);
        //    context.SaveChanges();
        //    return CreatedAtAction(nameof(GetDeptById), new { id = department.Id }, department);
        //}

        //Add Unique Name
        [Authorize(Roles ="Admin")]
        [HttpPost("AddDepartment")]
        public IActionResult AddDepartment(AddDepartmentDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (context.Departments.Any(a => a.Name == model.Name))
                return BadRequest("Department Is already Exist");
            var dept = new Department
            {
                Name = model.Name
            };
            context.Departments.Add(dept);
            context.SaveChanges();
            return Ok($" Department {model.Name} Added Successfully");
        }

        //Bulk Insert
        [Authorize(Roles = "Admin")]
        [HttpPost("BulkInsert")]
        public IActionResult BulkInsert([FromBody] List<BulkInsertDeptDTO> models)
        {
            if (models == null || !models.Any())
                return BadRequest(" no departmnet Added");
            var NewDepartments = new List<Department>();
            foreach( var model in models)
            {
                if (context.Departments.Any(f => f.Name == model.Name))
                    return BadRequest(" Department Is Already Exsist");
                var dept = new Department
                {
                   Name = model.Name
                };
                NewDepartments.Add(dept);
            }
            context.Departments.AddRange(NewDepartments);
            context.SaveChanges();
            return Ok(new
            {
                Message = $"{NewDepartments.Count} departments Added Successfully" ,Departments = NewDepartments
            });
        }

        //GetDepartmentWithEmployee By Id
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("{id:int}/Employee")]

        public async Task<IActionResult> GetDepartmentWithEmployee(int id)
        {
            var dept = await context.Departments
                .Where(c => c.Id == id && !c.IsDeleted)
                .Select(c => new DeptWithEmpDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    TotalEmployee = c.Employees.Count(),
                    EmpLoyees = c.Employees.Select(p => new EmpLoyeeDTO
                    {
                        Id = p.Id,
                        FullName = p.FullName,
                        Email = p.Email,
                        Salary = p.Salary,
                        HireDate = p.HireDate

                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (dept == null)
            {
                return NotFound(new { Message = "Departmnet not found" });
            }

            return Ok(dept);
        }

        //PutDepartment
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/EditDepartment")]
        public IActionResult EditDepartment(int id, [FromBody] EditDepartmentDTO model)
        {
            var res = context.Departments.Find(id);
            if (res == null)
            {
                return NotFound("Department Not Found");
            }
            if (context.Departments.Any(d => d.Name == model.NewName && d.Id != id))
                return BadRequest(" Department Already Exist");
            res.Name = model.NewName;
            context.SaveChanges();
            return Ok(res);
        }

        //Restore Department
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/Restore")]
        public IActionResult RestoreDept(int id)
        {
            var dept = context.Departments.FirstOrDefault(x => x.Id == id && x.IsDeleted ==  true);
            if (dept == null)
                return BadRequest(" No department found With This Id");
            dept.IsDeleted = false;
            context.SaveChanges();
            return Ok(new
            {
                Message =$" Department {dept.Name} Restord Successfully"
            });
        }

        //Delete Department(Soft Deleted)
        [HttpDelete("{id:int}/DeleteDepartment")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDepartment(int id) 
        {
            var res = context.Departments.Find(id);
            if (res == null)
               return NotFound("Department Not Found");
            //context.Departments.Remove(res);
            res.IsDeleted = true;
            context.SaveChanges();
            return Ok( new{
                message=$" Department {res.Name} Deleted Successfully "
            });
        }


        //Get Departnet Statistics(id)
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("{id:int}/Statistics")]
        public IActionResult GetDeptStatistics(int id)
        {
            var found = context.Departments.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
            if (found == null)
                return NotFound();
            var stats = new
            {
                TotalEmployees = found.Employees.Count(),
                 AverageSalary = found.Employees.Any() ? found.Employees.Average(e=>e.Salary) : 0
            };
            return Ok(stats);
        }

        //Get Departnet Statistics(name)
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("ByName/{name:alpha}/Statistics")]
        public IActionResult GetDeptStatistics(string name)
        {
            var found = context.Departments.FirstOrDefault(i => i.Name == name && !i.IsDeleted);
            if (found == null)
                return NotFound();
            var stats = new
            {
                TotalEmployees = found.Employees.Count(),
                AverageSalary = found.Employees.Any() ? found.Employees.Average(e => e.Salary) : 0
            };
            return Ok(stats);
        }

        //Search
        [HttpGet("Search")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> SearchDepartments(
        [FromQuery] string? keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Please provide a search keyword");
            }

            var query = context.Departments
                .Where(d=>d.Name.Contains(keyword) && !d.IsDeleted);

            var totalCount = await query.CountAsync();

            var departments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                data = departments
            });
        }

        //Summary
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("Summary")]
        public IActionResult Summary()
        {
            var res = context.Departments.Select(d => new
            {
                d.Id,
                d.Name,
                EmployeeCount = d.Employees.Count()
            }).ToList();

            return Ok(res);
        }

        //Get Top Department
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetTopDepartment")]
        public IActionResult GetTopDepartment([FromQuery] int count =3)
        {
            if (count <= 0) 
            {
                return BadRequest(" Count Must Be greater Than Zero");
            }
            var departments = context.Departments
                .Where(d => !d.IsDeleted)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    EmployeeCount = d.Employees.Count()
                })
                .OrderByDescending(d =>
                d.EmployeeCount)
                .Take(count)
                .ToList();
            if (!departments.Any())
                return BadRequest(" No Departments Found");
            return Ok(departments);
        }

        //GetDeletedDepartments
        [Authorize(Roles = "Admin")]
        [HttpGet("Deleted")]
        public IActionResult GetDeletedDepartments()
        {
            var res = context.Departments.Where(d => d.IsDeleted)
                .ToList();
            return Ok(res);
        }


    }
}
