//using Microsoft.AspNetCore.Mvc;
//using StudyMATEUpload.Models;
//using StudyMATEUpload.Repository.Generics;
//using StudyMATEUpload.Models.DTOs;
//using StudyMATEUpload.Models.ViewModels;
//using AutoMapper;
//using System.Threading.Tasks;
//using StudyMATEUpload.Enums;
//using System.Linq;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;

//namespace StudyMATEUpload.Controllers
//{
//    [Route("api/[controller]")]
//    public class LearnCoursesController : GenericController<LearnCourse, LearnCourseViewModel, LearnCourseDTO>
//    {
//        public LearnCoursesController(IModelManager<LearnCourse> repo, IMapper mapper): base(repo, mapper)
//        {}

//        [HttpGet("level")]
//        public async ValueTask<IActionResult> Get(StudyLevel level, int page=1)
//        {
//            int itemsPerPage = 20;
//            var models = await _repo
//                                .Item()
//                                .Where(c => c.StudyLevel == level)
//                                .Skip(page * itemsPerPage - itemsPerPage)
//                                .Take(itemsPerPage)
//                                .ToListAsync();
//            return Ok(_mapper.Map<ICollection<LearnCourse>, ICollection<LearnCourseDTO>>(models));
//        }


//        [HttpGet("{id:int}")]
//        public override async ValueTask<IActionResult> Get(int id)
//        {
//            var model = await _repo
//                                .Item()
//                                .Where(c => c.Id == id)
//                                .Include(u => u.Quizzes)
//                                    .ThenInclude(u => u.Options)
//                                .Include(u => u.Videos)
//                                .FirstOrDefaultAsync();
//            if (model != null)
//            {
//                return Ok(_mapper.Map<LearnCourse, LearnCourseDTO>(model));
//            }
//            return NotFound();
//        }
//    }
//}
