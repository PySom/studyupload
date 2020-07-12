using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyMATEUpload.Enums;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IModelManager<Test> _repo;
        private readonly IModelManager<Quiz> _quiz;
        private readonly IMapper _mapper;
        public TestsController(IModelManager<Test> repo, IMapper mapper, IModelManager<Quiz> quiz)
        {
            _repo = repo;
            _mapper = mapper;
            _quiz = quiz;
        }

        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            ICollection<Test> tests = await _repo
                                            .Item()
                                            .ToListAsync();
            return Ok(tests);

        }

        [HttpGet("{id:int}")]
        public async ValueTask<IActionResult> Get(int id, bool parent = false, StudyType study = StudyType.StudyMate)
        {
            if (parent)
            {
                if(study == StudyType.StudyMate)
                {
                    return Ok(await _repo.Item()
                   .Where(q => q.CourseId == id && q.StudyType == study)
                   .Include(t => t.Videos)
                   .Include(t => t.Quizes)
                       .ThenInclude(q => q.Options)
                   .ToListAsync());
                }
                return Ok(await _repo.Item()
                    .Where(q => q.CourseId == id && q.StudyType == study)
                    .Include(t => t.Videos)
                    .Include(t => t.Quizes.Where(q => q.IncludeThis))
                        .ThenInclude(q => q.Options)
                    .Include(t => t.UserTests)
                        .ThenInclude(u => u.UserQuizzes)
                    .Include(t => t.UserTests)
                        .ThenInclude(u => u.UserVideos)
                    .ToListAsync());
            }
            Test model = await _repo
                                .Item()
                                .Where(c => c.Id == id)
                                .FirstOrDefaultAsync();
            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [HttpGet("sane/{id:int}")]
        public async ValueTask<IActionResult> GetSane(int id, StudyType study = StudyType.StudyMate)
        {
            var model = await _repo.Item()
                    .Where(q => q.Id == id && q.StudyType == study)
                    .Include(t => t.Quizes.Where(q => q.IncludeThis))
                        .ThenInclude(q => q.Options)
                    .Include(t => t.UserTests)
                        .ThenInclude(u => u.UserQuizzes)
                    .FirstOrDefaultAsync();
            if (model == null) return NotFound();
            return Ok(model);
        }

        [HttpGet("current/{id:int}")]
        public async ValueTask<IActionResult> Get(int id, StudyType study = StudyType.StudyMate)
        {
            Test model = null;
            if (study == StudyType.StudyMate)
            {
                model = await _repo.Item()
                   .Where(q => q.Id == id && q.StudyType == study)
                   .Include(t => t.Quizes)
                       .ThenInclude(q => q.Options)
                   .FirstOrDefaultAsync();
            }
            else
            {
                model = await _repo.Item()
                    .Where(q => q.Id == id && q.StudyType == study)
                    .Include(t => t.Videos)
                    .Include(t => t.Quizes)
                        .ThenInclude(q => q.Options)
                    .Include(t => t.UserTests)
                        .ThenInclude(u => u.UserQuizzes)
                    .Include(t => t.UserTests)
                        .ThenInclude(u => u.UserVideos)
                    .FirstOrDefaultAsync();
            }
            
            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async ValueTask<IActionResult> Post([FromBody] Test model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, Test test, string error) = await _repo.Add(model);
                if (succeeded) return Ok(test);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }

        [HttpPut]
        public async ValueTask<IActionResult> Put([FromBody] Test model)
        {
            if (ModelState.IsValid)
            {
                (bool succeeded, Test test, string error) = await _repo.Update(model);
                if (succeeded) return Ok(test);
                return BadRequest(new { Message = error });
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }


        [HttpDelete("{id}")]
        public async ValueTask<IActionResult> Delete(int id)
        {
            Test test = new Test { Id = id };
            string message;
            try
            {
                var quizes = _quiz.Item().Where(q => q.TestId == id);
                var _ = await _quiz.Delete(quizes);
                (bool succeeded, string error) = await _repo.Delete(test);
                message = error;
                if (succeeded) return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                message = ex.Message;
            }
            return NotFound(new { Message = message });
        }
    }
}