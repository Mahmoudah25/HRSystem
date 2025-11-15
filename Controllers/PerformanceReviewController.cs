using HRManagmentSystem.DTOs.PerformanceReview;
using HRManagmentSystem.Models;
using HRManagmentSystem.Models.Traning_Performance;
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
    public class PerformanceReviewController : ControllerBase
    {
        private readonly HRContext context;
        public PerformanceReviewController(HRContext context)
        {
            this.context = context;
        }

        //GetAllReviews
        [HttpGet("GetAll")]
        public IActionResult GetAllReviews()
        {
            var res = context.PerformanceReviews.Include(x => x.Employee).ToList();
            if (!res.Any())
                return NotFound();
            return Ok(res);
        }

        //GetReviewById
        [HttpGet("GetById/{id:int}")]
        public IActionResult GetById(int id)
        {
            var found = context.PerformanceReviews.FirstOrDefault(x => x.Id == id);
            if (found == null)
                return NotFound();
            return Ok(found);
        }

        //GetReviewsByEmployee
        [HttpGet("getByEmployee/{empId:int}")]
        public IActionResult GetReviewsByEmployee(int empId)
        {
            var found = context.PerformanceReviews.Where(x => x.EmployeeId == empId).ToList();
            if (!found.Any())
                return NotFound(" No Review for this  Employee Found");
            return Ok(found);
        }

        //Add
        [HttpPost("Add")]
        public IActionResult Add([FromBody ] AddPerformanceReviewDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var Existemp = context.Employees.Any(e => e.Id == model.EmployeeId);
            if(!Existemp)
                return NotFound(" Employee Not Found");
            var performance = new PerformanceReview
            {
                ReviewDate = model.ReviewDate,
                Comments = model.Comments,
                Score = model.Score,
                EmployeeId = model.EmployeeId
            };
            context.PerformanceReviews.Add(performance);
            context.SaveChanges();
            return Ok(performance);
        }

        //Edit
        [HttpPut("Edit/{id:int}")]
        public IActionResult Edit(int id , EditPerformanceReviewDTO model )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var found = context.PerformanceReviews.Find(id);
            if(found == null)
                return NotFound();
            found.ReviewDate = model.ReviewDate;
            found.Comments = model.Comments;
            found.Score = model.Score;
            context.SaveChanges();
            return Ok(found);
        }

        //Delete
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id:int}")]
        public IActionResult Delete(int id) 
        {
            var found = context.PerformanceReviews.Find(id);
            if (found == null)
                return NotFound();
            context.PerformanceReviews.Remove(found);
            context.SaveChanges();
            return Ok(found);
        }

        //GetinDateRange
        [HttpGet("ByDateRange")]
        public IActionResult GetinDateRange(DateTime start , DateTime End) 
        {
            var found  = context.PerformanceReviews.Where( x=>x.ReviewDate >= start && x.ReviewDate <= End)
                .ToList();
            if(!found.Any())
                return NotFound(" No Review Data In This Date Range");
            return Ok(found);
        }

        //GetLatestRewiew For Employee
        [HttpGet("Latest/{empId:int}")]
        public IActionResult GetLatestRewiewForEmployee(int empId) 
        {
            var found = context.PerformanceReviews.Where(x=>x.EmployeeId == empId)
                 .OrderByDescending(x=>x.ReviewDate)
                 .FirstOrDefault();
            if(found == null)
                return NotFound();
            return Ok(found);
        }

        //GetAverageScoreFroEmployee
        [HttpGet("AverageScore/{empId:int}")]
        public IActionResult GetAverageScoreFroEmployee(int empId) 
        {
            var found = context.PerformanceReviews.Where(x => x.EmployeeId == empId)
                .ToList();
            if (!found.Any())
                return NotFound("No Review for this  Employee Found");
            var avg = found.Average( x => x.Score );
            return Ok(avg);
        }

        //GetByScoreRange
        [HttpGet("ByScoreRange")]
        public IActionResult GetByScoreRange(double minScore ,double maxScore)
        {
            var found = context.PerformanceReviews.Where(x => x.Score >= minScore && x.Score <= maxScore)
                .ToList();
            if (!found.Any())
                return NotFound("No Review for this  Range Found");
            return Ok(found);
        }

        //GetTopReviews
        [HttpGet("Top/{empId:int}")]
        public IActionResult GetTopReviews(int empId , int count =3)
        {
            var found = context.PerformanceReviews
                .Where(x => x.EmployeeId == empId)
                .OrderByDescending(x => x.Score)
                .Take(count)
                .ToList();
            if (!found.Any())
                return NotFound("No Review Found");
            return Ok(found);
        }

        //CountByemployee
        [HttpGet("Count/{empId:int}")]
        public IActionResult CountByemployee(int empId)
        {
            var count = context.PerformanceReviews
                .Count(x => x.EmployeeId == empId);
            return Ok(count);
        }

        //GEtEmployeesWithoutReviews
        [HttpGet("NoReviews")]
        public IActionResult GEtEmployeesWithoutReviews()
        {
            var employees = context.Employees.
                Where(e => !context.PerformanceReviews.Any(x => x.EmployeeId == e.Id))
                .ToList();
            if (!employees.Any())
                return NotFound("All Employee Have Reviews");
            return Ok(employees);
        }

        //OrderByDate
        [HttpGet("OrderByDate")]
        public IActionResult GetOrderedByDate(string order = "desc")
        {
            var found = order.ToLower() == "asc"
                ? context.PerformanceReviews.OrderBy(x => x.ReviewDate).ToList()
                : context.PerformanceReviews.OrderByDescending(x => x.ReviewDate).ToList();

            return Ok(found);
        }

        //GetLatestReviewDateForAllEmployees
        [HttpGet("LatestForAll")]
        public IActionResult GetLatestReviewDateForAllEmployees()
        {
            var latest = context.PerformanceReviews
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.OrderByDescending(r => r.ReviewDate).FirstOrDefault())
                .ToList();
            return Ok(latest);
        }

        //FilterComment(Search)
        [HttpGet("SearchComment")]
        public IActionResult FilterComment(string Keyword)
        {
            var res = context.PerformanceReviews
                .Where(x => x.Comments.Contains(Keyword.ToLower()))
                .ToList();
            if (!res.Any())
                return NotFound(" No Reviews Mateched KeyWord");
            return Ok(res);
        }

        //GetAverageScoreAll
        //AverageScoresAll
        [HttpGet("AverageScoresAll")]
        public IActionResult GetAverageScoresAll()
        {
            var avgScores = context.PerformanceReviews
                .GroupBy(x => x.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    AverageScore = g.Average(x => x.Score)
                })
                .ToList();

            return Ok(avgScores);
        }
    }
}
