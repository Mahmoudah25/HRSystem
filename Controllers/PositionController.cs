using HRManagmentSystem.DTOs.Position;
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
    public class PositionController : ControllerBase
    {
        private readonly HRContext context;
        public PositionController(HRContext context)
        {
            this.context = context;
        }

        //GetAllPositons
        [HttpGet("GetAllPositons")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult GetAllPositons()
        {
            var res=context.Positions.ToList();
            return Ok(res);
        }

        //GetPositionById
        [HttpGet("GetPositonsById")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult GetPositionById(int id)
        {
            var found = context.Positions.FirstOrDefault(x => x.Id == id);
            if (found != null) 
                return Ok(found);
            return NotFound("Position Not Found");
        }

        //GetPositionByTitle
        [HttpGet("GetPositionByTitle")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult GetPositionByTitle(string name)
        {
            var found = context.Positions.FirstOrDefault(x => x.Title == name);
            if (found != null)
                return Ok(found);
            return NotFound("Position Not Found");
        }

        //AddPosition
        [HttpPost("AddPosition")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult AddPosition([FromBody] AddPoistionDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var position = new Position
            {
                Title = model.Title,
                Level = model.Level
            };
            context.Positions.Add(position);
            context.SaveChanges();
            return Ok(position);
        }

        //EditPosition
        [HttpPut("{id:int}/EditPosition")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult EditPosition(int id , EditPositionDTO model)
        {
            var found = context.Positions.Find(id);
            if (found != null) 
            {
                found.Title = model.Title;
                found.Level = model.Level;
                return Ok(found);
            }
            context.SaveChanges();
            return NotFound(" Poistion Not Found");
        }

        //DeletePosition
        [HttpDelete("{id:int}/DeletePosition")]
        [Authorize(Roles = "Admin,Customer")]
        public IActionResult DeletePosition(int id) 
        {
            var found = context.Positions.Find(id);
            if (found != null)
            {
                context.Positions.Remove(found);
                context.SaveChanges();
                return Ok($"Position{found.Title} Deleted Successfully");
            }
            return NotFound(" Poistion Not Found");
        }

        //Search
        [HttpGet("Search")]
        public IActionResult Search( string? title , string? level)
        {
            var query = context.Positions.AsQueryable();
            if (!string.IsNullOrEmpty(title))
                query = query.Where(p => p.Title.Contains(title));
            if(!string.IsNullOrEmpty(level))
                query = query.Where(p => p.Level == level);
            return Ok(query.ToList());
        }
    }
}
