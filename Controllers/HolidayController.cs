using HRManagmentSystem.DTOs.HoliDay;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HolidayController : ControllerBase
    {
        private readonly HRContext context;
        public HolidayController(HRContext context)
        {
            this.context = context;
        }

        //GetAllHolidays
        [HttpGet("GetAllHolidays")]
        public IActionResult GetAllHolidays()
        {
            var res = context.Holidays.ToList();
            if (!res.Any())
                return NoContent();
            return Ok(res);
        }

        //GetHolidayByID
        [HttpGet("GetByID/{id:int}")]
        public IActionResult GetHolidayByID(int id)
        {
            var found = context.Holidays.FirstOrDefault(x => x.Id == id);
            if (found == null)
                return NotFound("Holiday Not Found");
            return Ok(found);
        }

        //GetHolidayByName
        [HttpGet("GetByName/{name:alpha}")]
        public IActionResult GetHolidayByName(string name)
        {
            var found = context.Holidays.FirstOrDefault(x => x.Name == name);
            if (found == null)
                return NotFound("Holiday Not Found");
            return Ok(found);
        }

        //GetHolidayByDate
        [HttpGet("GetByDate/{date:datetime}")]
        public IActionResult GetHolidayByDate(DateTime date)
        {
            var found = context.Holidays.FirstOrDefault(x => x.Date == date);
            if (found == null)
                return NotFound("Holiday Not Found");
            return Ok(found);
        }

        //GetRecurring
        [HttpGet("GetRecurring")]
        public IActionResult GetRecurring()
        {
            var recuuring = context.Holidays.Where( h => h.IsRecurring ).ToList();
            if(!recuuring.Any())
                return NoContent();
            return Ok(recuuring);
        }

        //AddHoliday
        [HttpPost("AddHoliday")]
        public IActionResult AddHoliday([FromBody] AddHoildayDTO model)
        {
            var Exsit = context.Holidays.Any( h=>h.Name == model.Name && h.Date == model.Date);
            if(Exsit)
                return BadRequest("Holiday Already Exist");
            var holiday = new Holiday
            {
                Name = model.Name,
                Date = model.Date,
                IsRecurring = model.IsRecurring,
            };
            context.Holidays.Add(holiday);
            context.SaveChanges();
            return CreatedAtAction(nameof(GetHolidayByID) , new { id =holiday.Id } , holiday);
        }

        //EditHoliday
        [HttpPut("EditHoliday/{id:int}")]
        public IActionResult Edit( int id , [FromBody] EditHoliDayDTO model)
        {
            var found = context.Holidays.Find(id);
            if (found == null)
                return NotFound();
            found.Name = model.Name;
            found.Date = model.Date;
            found.IsRecurring = model.IsRecurring;
            context.SaveChanges();
            return Ok();

        }

        //DeleteHoliday
        [HttpDelete("DeleteHoliday/{id:int}")]
        public IActionResult DeleteHoliday(int id) 
        {
            var found = context.Holidays.Find(id);
            if(found == null)
                return NotFound();
            context.Holidays.Remove(found);
            context.SaveChanges();
            return Ok();
        }

        //GetUpcomingHolidays
        [HttpGet("Upcoming")]
        public IActionResult GetUpcomingHolidays() 
        {
            var today = DateTime.Today;
            var res = context.Holidays.Where(x=>x.Date >= today)
                .OrderBy(x=>x.Date).ToList();
            if(!res.Any())
                return NotFound();
            return Ok(res);
        }

        //GetInDateRange
        [HttpGet("Range")]
        public IActionResult GetInDateRange(DateTime start, DateTime end)
        {
            var res = context.Holidays.Where(x=>x.Date >= start && x.Date <=end)
                .ToList();
            if(!res.Any())
                return NotFound();
            return Ok(res);
        }

        //Count
        [HttpGet("Count")]
        public IActionResult count()
        {
            var res = context.Holidays.Count();
            return Ok(new
            {
                TotalHoliDays = res
            });
        }
    }
}
