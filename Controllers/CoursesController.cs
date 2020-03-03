using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StudyMATEUpload.Models;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Repository.Extension;
using StudyMATEUpload.Repository.Generics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IModelManager<Course> _repo;
        private readonly IMapper _mapper;
        public CoursesController(IModelManager<Course> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            ICollection<Course> courses = await _repo
                                                .Item()
                                                .Include(c => c.Tests)
                                                    .ThenInclude(c => c.Quizes)
                                                        .ThenInclude(q => q.Options)
                                                .ToListAsync();
            return Ok(courses);
            
        }

        [HttpGet("{id:int}")]
        public async ValueTask<IActionResult> Get(int id)
        {
            CourseViewModel model = await _repo
                                            .Item()
                                            .Where(c => c.Id == id)
                                            .Select(c => c.Convert<Course, CourseViewModel>(_mapper))
                                            .FirstOrDefaultAsync();
            if(model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async ValueTask<IActionResult> Post([FromBody] Course model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, Course course, string error) = await _repo.Add(model);
                if (succeeded) return Ok(course);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPut]
        public async ValueTask<IActionResult> Put([FromBody] Course model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, Course nc, string error) = await _repo.Update(model);
                if (succeeded) return Ok(nc);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }
        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> Delete(int id)
        {
            Course course = new Course { Id = id };
            string message;
            try
            {
                (bool succeeded, string error) = await _repo.Delete(course);
                message = error;
                if (succeeded) return NoContent();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                message = ex.Message;
            }
            return NotFound(new { Message =  message });
        }
    }
}