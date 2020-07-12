//using Microsoft.AspNetCore.Mvc;
//using StudyMATEUpload.Models;
//using StudyMATEUpload.Repository.Generics;
//using StudyMATEUpload.Models.ViewModels;
//using StudyMATEUpload.Models.DTOs;
//using AutoMapper;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;

//namespace StudyMATEUpload.Controllers
//{
//    [Route("api/[controller]")]
//    public class UserLearnCoursesController : GenericController<UserLearnCourse, UserLearnCourseViewModel, UserLearnCourseDTO>
//    {
//        public UserLearnCoursesController(IModelManager<UserLearnCourse> repo, IMapper mapper) : base(repo, mapper)
//        { }

//        [HttpGet("withparams")]
//        public async ValueTask<IActionResult> GetAll(int page = 1, bool includeChildren = false, string which = null)
//        {
//            int itemsPerPage = 20;
//            if (includeChildren)
//            {
//                if(which != null) 
//                {
//                    if(which == "videos")
//                    {
//                        return Ok(
//                            await _repo.Item()
//                            .Skip(page * itemsPerPage - itemsPerPage)
//                            .Take(itemsPerPage)
//                            .Include(u => u.UserQuizzes)
//                            .ToListAsync()
//                            );
//                    }
//                    else if (which == "videos")
//                    {
//                        return Ok(
//                            await _repo.Item()
//                            .Skip(page * itemsPerPage - itemsPerPage)
//                            .Take(itemsPerPage)
//                            .Include(u => u.UserVideos)
//                            .ToListAsync()
//                            );
//                    }
//                    else if (which == "both")
//                    {
//                        return Ok(
//                            await _repo.Item()
//                            .Skip(page * itemsPerPage - itemsPerPage)
//                            .Take(itemsPerPage)
//                            .Include(u => u.UserVideos)
//                            .Include(u => u.UserQuizzes)
//                            .ToListAsync()
//                            );
//                    }
//                }
//            }
//            return await base.GetAll(page);
//        }

//        [HttpPost]
//        public override async ValueTask<IActionResult> Post([FromBody] UserLearnCourseViewModel model)
//        {
//            var courseAdded = await _repo.Item().AnyAsync(m => m.LearnCourseId == model.LearnCourseId && m.UserId == model.UserId);
//            if (courseAdded) return BadRequest(new { Message = "You already have this to your list of courses" });
//            return await base.Post(model);
//        }

        
//    }
//}
