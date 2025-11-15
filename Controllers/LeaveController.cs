using HRManagmentSystem.DTOs.Leave;
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
    public class LeaveController : ControllerBase
    {
        private readonly HRContext context;
        public LeaveController(HRContext context)
        {
            this.context = context;
        }

        ////RequestLeave
        //[HttpPost("RequestLeave")]
        //public IActionResult RequestLeave([FromBody] Leave model)
        //{
        //    model.StartDate = "Pending";
        //}

        // Get all leaves
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult GetAllLeaves()
        {
            var leaves = context.Leaves
                                .Include(l => l.Employee)
                                .ToList();
            return Ok(leaves);
        }

        // Get leaves for a specific employee
        [HttpGet("GetByEmployee/{employeeId:int}")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult GetLeavesByEmployee(int employeeId)
        {
            var leaves = context.Leaves
                                .Include(l => l.Employee)
                                .Where(l => l.EmployeeId == employeeId)
                                .ToList();
            return Ok(leaves);
        }

        // . Request new leave
        [HttpPost("RequestLeave")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult RequestLeave([FromBody] AddLeaveDTO model)
        {
            // 🔸 Prevent overlapping leave
            var overlap = context.Leaves.Any(l =>
                l.EmployeeId == model.EmployeeId &&
                l.Status != LeaveStatus.Rejected &&
                (
                    (model.StartDate >= l.StartDate && model.StartDate <= l.EndDate) ||
                    (model.EndDate >= l.StartDate && model.EndDate <= l.EndDate)
                )
            );

            if (overlap)
                return BadRequest("Leave request overlaps with an existing leave.");

            //model.Status = LeaveStatus.Pending;
            //context.Leaves.Add(model);
            var newLeave = new Leave
            {
                LeaveType=model.LeaveType,
                EmployeeId = model.EmployeeId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = LeaveStatus.Pending,
                Reason= model.Reason
            };
            context.Leaves.Add(newLeave);
            context.SaveChanges();
            return Ok(model);
        }

        //  Approve leave
        [HttpPut("{id:int}/Approve")]
        [Authorize(Roles = "Admin")]
        public IActionResult ApproveLeave(int id)
        {
            var leave = context.Leaves.Find(id);
            if (leave == null)
                return NotFound("Leave not found.");

            leave.Status = LeaveStatus.Approved;
            context.SaveChanges();
            return Ok(leave);
        }
        //  Reject leave
        [HttpPut("{id:int}/Reject")]
        [Authorize(Roles = "Admin")]
        public IActionResult RejectLeave(int id)
        {
            var leave = context.Leaves.Find(id);
            if (leave == null)
                return NotFound("Leave not found.");

            leave.Status = LeaveStatus.Rejected;
            context.SaveChanges();
            return Ok(leave);
        }

        // Filter leaves by status (Pending / Approved / Rejected)
        [HttpGet("FilterByStatus")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult FilterByStatus(LeaveStatus status)
        {
            var leaves = context.Leaves
                                .Include(l => l.Employee)
                                .Where(l => l.Status == status)
                                .ToList();
            return Ok(leaves);
        }

        //Filter leaves by date range
        [HttpGet("FilterByDateRange")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult FilterByDateRange(DateTime startDate, DateTime endDate)
        {
            var leaves = context.Leaves
                                .Include(l => l.Employee)
                                .Where(l => l.StartDate >= startDate && l.EndDate <= endDate)
                                .ToList();
            return Ok(leaves);
        }

        // Delete leave (optional)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteLeave(int id)
        {
            var leave = context.Leaves.Find(id);
            if (leave == null)
                return NotFound("Leave not found.");

            context.Leaves.Remove(leave);
            context.SaveChanges();
            return Ok(leave);
        }

        //GetLeaveById
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult GetLeaveById(int id)
        {
            var leav = context.Leaves.Include(i => i.Employee)
                .FirstOrDefault(x => x.Id == id);
            if (leav == null)
                return NotFound(" Leave Not Found");
            return Ok(leav);
        }

        //update Leave
        [HttpPut("Update{id:int}")]
        [Authorize(Roles = "Admin,Employee")]
        public IActionResult updateLeave(int id , [FromBody] EditLeaveDTO model)
        {
            var leav = context.Leaves.Find(id);
            if (leav == null)
                return NotFound(" Leave Not Found");
            if (leav.Status != LeaveStatus.Pending)
                return BadRequest(" Only pending Leaves can be updated");
            leav.LeaveType = model.LeaveType;
            leav.StartDate = model.StartDate;
            leav.EndDate = model.EndDate;
            leav.Reason = model.Reason;

            context.SaveChanges();
            return Ok(leav);
        }

        //GetLeaevesByDate
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("ByDate")]
        public IActionResult GetLeaevesByDate(DateTime date)
        {
            var leaves = context.Leaves.Include(i => i.Employee)
                .Where(x => x.StartDate <= date && x.EndDate >= date)
                .ToList();
            return Ok(leaves);
        }

        //GetPendingleaves
        [HttpGet("Pending")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetPendingleaves()
        {
            var leaves = context.Leaves.Include(i => i.Employee)
                .Where(x => x.Status == LeaveStatus.Pending)
                .ToList();
            return Ok(leaves);
        }
    }
}
