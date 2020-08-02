using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserTestsController : GenericController<UserTest, UserTestViewModel, UserTestDTO>
    {
        public UserTestsController(IModelManager<UserTest> repo, IMapper mapper): base(repo, mapper)
        {}

        [HttpPost]
        public override async ValueTask<IActionResult> Post([FromBody] UserTestViewModel model)
        {
            var courseAdded = await _repo.Item().FirstOrDefaultAsync(m => m.TestId == model.TestId && m.UserCourseId == model.UserCourseId);
            if (courseAdded is object)
            {
                return Ok(courseAdded);
            }
            return await base.Post(model);
        }
    }
}
