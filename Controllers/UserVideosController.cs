using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserVideosController : GenericController<UserVideo, UserVideoViewModel, UserVideoDTO>
    {
        public UserVideosController(IModelManager<UserVideo> repo, IMapper mapper): base(repo, mapper)
        {}

        [HttpPost]
        public override async ValueTask<IActionResult> Post([FromBody] UserVideoViewModel model)
        {
            var data = await _repo.Item().Where(uv => uv.UserTestId == model.UserTestId && uv.VideoId == model.VideoId).FirstOrDefaultAsync();
            if(data != null)
            {
                data.Duration = model.Duration;
                return await Put(data);
            }
            return await base.Post(model);
        }
    }
}
