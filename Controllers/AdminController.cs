using HRManagmentSystem.DTOs.Account_User;
using HRManagmentSystem.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        //Add Admin
        [HttpPost("Add-Admin")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddAdmin([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await userManager.AddToRoleAsync(user, "Admin");
            return Ok($"Admin {user.UserName} Created Successfully");
        }

        //AddEmployee
        [HttpPost("Add-Employee")]
        //[Authorize(Policy = "RequireHRRole")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> AddEmployee([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            await userManager.AddToRoleAsync(user, "Employee");
            return Ok($"Employee {user.UserName} Created Successfully");
        }

        //AddHR
        [HttpPost("Add-HR")]
        //[Authorize(Policy = "RequireAdminRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddHR([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            await userManager.AddToRoleAsync(user, "HR");
            return Ok($" HR {user.UserName} Created Successfully");
        }

        //Profile
        [HttpGet("Profile")]
        //[Authorize(Policy = "RequireEmployeeRole")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();
            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email
            });
        }

        //
        [HttpGet("CheckRole")]
        [Authorize]
        public async Task<IActionResult> CheckRole([FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.GetUserAsync(User);
            var roles = await userManager.GetRolesAsync(user);
            return Ok(roles);
        }
    }
}
