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
using System;
using StudyMATEUpload.Enums;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IModelManager<Course> _repo;
        private readonly IModelManager<Quiz> _quiz;
        private readonly IModelManager<Option> _option;
        private readonly IModelManager<Test> _test;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> names = 
            new Dictionary<string, string>{
                { "crs", "" },
                { "bio", "Q" },
                { "chem", "A" },
                { "econ", "Q" },
                { "use", "Q" },
                { "geo", "Q" },
                { "gov", "Q" },
                { "phy", "q" },
                { "comm", "Q" },
                { "acc", "Q" },
                { "lit", "q" },
                {"math", "Q" }
            };
        public CoursesController(IModelManager<Course> repo,
            IModelManager<Quiz> quiz,
            IModelManager<Option> option,
            IModelManager<Test> test,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _quiz = quiz;
            _option = option;
            _test = test;
        }


        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            ICollection<Course> courses = await _repo
                .Item()
                .Where(c => c.HasStudyPack)
                .ToListAsync();
            return Ok(courses);
            
        }

        [HttpGet("{id:int}")]
        public async ValueTask<IActionResult> Get(int id)
        {
            Course course = await _repo
                .Item()
                .Where(c => c.HasMain && c.Id == id)
                .Include(c => c.Tests.Where(t => t.StudyType == StudyType.Main))
                    .ThenInclude(u => u.Quizes)
                         .ThenInclude(u => u.Options)
                .Include(c => c.Tests.Where(t => t.StudyType == StudyType.Main))
                    .ThenInclude(u => u.Videos)
                .FirstOrDefaultAsync();
            if (course == null) return NotFound();
            return Ok(course);

        }

        [HttpGet("level/{level}")]
        public async ValueTask<IActionResult> GetCourseByLevel(StudyLevel level)
        {
            ICollection<Course> courses = await _repo
                    .Item()
                    .Where(c => level == c.StudyLevel)
                    .ToListAsync();

            return Ok(courses);

        }

        [HttpGet("alllevels")]
        public async ValueTask<IActionResult> GetCourseByAllLevels()
        {
            ICollection<Course> courses = await _repo
                    .Item()
                    .Where(c => c.HasMain && c.IsVisible)
                    .ToListAsync();

            return Ok(courses);

        }

        [HttpGet("all")]
        public async ValueTask<IActionResult> GetAll()
        {
            ICollection<Course> courses = await _repo
                .Item()
                .Where(c => c.HasStudyPack)
                .Include(c => c.Tests.Where(t => t.StudyType == StudyType.StudyMate))
                    .ThenInclude(e => e.Quizes.OrderBy(p => p.QuestionNumber))
                        .ThenInclude(f => f.Options)
                .ToListAsync();

            //var model = courses.Select(u => new Course
            //{
            //    Id = u.Id,
            //    Name = u.Name,
            //    SubText = u.SubText,
            //    Tests = u.Tests.Where(t => t.StudyType == StudyType.StudyMate).Select(b => new Test
            //    {
            //        Id=b.Id,
            //        Course=b.Course,
            //        CourseId=b.CourseId,
            //        Duration=b.Duration,
            //        QuestionNo=b.QuestionNo,
            //        ShortDescription=b.ShortDescription,
            //        Text=b.Text,
            //        Year=b.Year,
            //        Quizes = b.Quizes.OrderBy(p => p.QuestionNumber).ToList()
            //    }).ToList() 
            //}).ToList();
            return Ok(courses);

        }


        [HttpGet("byname")]
        public async ValueTask<IActionResult> Get(string name, StudyType study = StudyType.StudyMate)
        {
            var model = await _repo
                                .Item()
                                .Where(c => c.Name.ToLower().Contains(name.ToLower()) && c.HasStudyPack)
                                .Include(c => c.Tests.Where(t => t.StudyType == study))
                                .FirstOrDefaultAsync();
            if (model != null)
            {
                return Ok(model);
            }
            return NotFound();
        }

        [HttpPut("updatealpha/{id}")]
        public async ValueTask<IActionResult> RemoveAlpha(int id)
        {
            if(id != 0)
            {
                var options = _option.Item()
                    .Include(o => o.Practice)
                        .ThenInclude(p => p.Test)
                    .Where(o => o.Practice.Test.CourseId == id)
                    .ToList();
                if (options.Count > 0)
                {
                    foreach (var option in options)
                    {
                        if (!string.IsNullOrEmpty(option.Content) && (option.Content.StartsWith("A.") ||
                            option.Content.StartsWith("B.") || option.Content.StartsWith("C.") || option.Content.StartsWith("D.")))
                        {
                            var newContent = option.Content[2..].Trim();
                            option.Content = newContent;
                            await _option.Update(option);
                        }
                    }
                    return NoContent();
                }
                return NotFound();
            }
            return BadRequest(new { Message = "get out" });
        }

        [HttpPost("updateaudio")]
        public async ValueTask<IActionResult> Post()
        {
            foreach(var name in names)
            {
                Course course = await _repo
                                        .Item()
                                        .Where(c => c.Name.ToLower().Contains(name.Key) && c.HasStudyPack)
                                        .Include(q => q.Tests.Where(t => t.StudyType == StudyType.StudyMate))
                                            .ThenInclude(t => t.Quizes)
                                        .FirstOrDefaultAsync();
                if(course != null)
                {
                    foreach (var test in course.Tests)
                    {
                        int index = 1;
                        foreach (var quiz in test.Quizes)
                        {
                            var yearAvailable = name.Key == "chem" ? $"Chem {test.Year} (" : "";
                            var closeBracket = name.Key == "chem" ? ")" : "";
                            quiz.AudioUrl = $"audios/{course.Name.ToLower().Replace(' ', '_')}/{test.Year}/{yearAvailable}{name.Value}{index}{closeBracket}.mp3";
                            index += 1;
                            await _quiz.Update(quiz);
                        }
                    }
                    
                }

            }

            return NoContent();
            
        }

        [HttpPost("updatequestionnumber")]
        public async ValueTask<IActionResult> PostNumber()
        {
            foreach (var name in names)
            {
                Course course = await _repo
                                        .Item()
                                        .Where(c => c.Name.ToLower().Contains(name.Key))
                                        .Include(q => q.Tests.Where(t => t.StudyType == StudyType.StudyMate))
                                            .ThenInclude(t => t.Quizes)
                                        .FirstOrDefaultAsync();
                if (course != null)
                {
                    foreach (var test in course.Tests)
                    {
                        int index = 1;
                        foreach (var quiz in test.Quizes)
                        {
                            quiz.QuestionNumber = index;
                            await _quiz.Update(quiz);
                            index++;
                        }
                    }

                }

            }

            return NoContent();

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
                var tests = _test.Item().Where(t => t.CourseId == id).Include(t => t.Quizes);
                foreach (var test in tests)
                {
                    await _quiz.Delete(test.Quizes);
                    var _ = await _test.Delete(test);
                }
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