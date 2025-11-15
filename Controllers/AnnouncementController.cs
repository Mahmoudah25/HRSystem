using HRManagmentSystem.DTOs.Allowance;
using HRManagmentSystem.DTOs.Announcement;
using HRManagmentSystem.Models;
using HRManagmentSystem.Models.Communication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AnnouncementController : ControllerBase
    {
        private readonly HRContext context;
        public AnnouncementController(HRContext context)
        {
            this.context = context;
        }

    

        [HttpGet("GetAll")]
        public IActionResult GetAllAnnouncements(int page = 1, int pageSize = 10)
        {
            var query = context.Announcements.AsQueryable();

            var res = query
                .Where(a=>!a.IsDeleted)
                .OrderByDescending(a => a.CraeteAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!res.Any())
                return NotFound("No announcements found.");

            return Ok(res);
        }

        //GetById
        [HttpGet("GetById/{id:int}")]
        public IActionResult GetById(int id)
        {
            var found = context.Announcements.FirstOrDefault(c => c.Id == id);
            if (found == null)
                return NotFound();
            return Ok(found);
        }

        //GetByName
        [HttpGet("GetByName/{name:alpha}")]
        public IActionResult GetByName(string name)
        {
            var found = context.Announcements.FirstOrDefault(c => c.Title == name);
            if (found == null)
                return NotFound();
            return Ok(found);
        }

        //AddAnnouncement
        [Authorize(Roles = "Admin,HR")]
        [HttpPost("Add")]
        public IActionResult Add([FromBody] AddAnnouncememtDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var anno = new Announcement
            {
                Title = model.Title,
                Message = model.Message,
                CraeteAt = DateTime.Now   
            };

            context.Announcements.Add(anno);
            context.SaveChanges();
            return Ok(anno);
        }

        //EditAnnouncement
        [Authorize(Roles = "Admin,HR")]
        [HttpPut("Edit/{id:int}")]
        public IActionResult Edit( int id , [FromBody] EditAnnouncememtDTO model)
        {
            var found = context.Announcements.Find(id);
            if (found == null)  
                return NotFound();
            found.Title = model.Title;
            found.Message = model.Message;
            context.SaveChanges();
            return Ok(found);
        }

        //Delete
        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id) 
        {
            var found = context.Announcements.Find(id);
            if (found == null)
                return NotFound();
            //context.Remove(found);
            found.IsDeleted= true;
            context.SaveChanges();
            return Ok($"Announcement {found.Title} Deleted Success");
        }

        //GetLatestAnnoucement
        [HttpGet("latest")]
        public IActionResult GetLatest() 
        {
            var res = context.Announcements.OrderByDescending(x=>x.CraeteAt)
                .FirstOrDefault();
            if(res == null)
                return NotFound(" No Annoucement Avalible ");
            return Ok(res);
        }

        //GetByDateRange
        [Authorize(Roles = "Admin,HR")]
        [HttpGet("GetByDateRange")]
        public IActionResult GetByDateRange(DateTime startDate , DateTime endDate)
        {
            var res = context.Announcements.Where(x => x.CraeteAt >= startDate && x.CraeteAt <= endDate)
                .OrderByDescending(a => a.CraeteAt)
                .ToList();
            if (!res.Any())
                return NotFound(" No Annoucement At This Range ");
            return Ok(res);
        }

        //Search
        [HttpGet("Search")]
        public IActionResult Search(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var res = context.Announcements.Where(a => a.Title.Contains(keyword) || a.Message.Contains(keyword))
                    .ToList();
                if (!res.Any())
                    return NotFound("No Annoucement Mateched");
                return Ok(res);
            }
            return BadRequest(" Keyword Is Reqiured");
        }

        //count
        [HttpGet("Count")]
        [Authorize(Roles = "Admin,HR")]
        public IActionResult Count()
        {
            var res = context.Announcements.Count(a=>!a.IsDeleted);
            return Ok(new
            {
                Total =res
            });

        }



    }
}
