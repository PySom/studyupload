using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;
using System.Threading.Tasks;
using StudyMATEUpload.Enums;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class VideosController : GenericController<Video, VideoViewModel, VideoDTO>
    {
        public VideosController(IModelManager<Video> repo, IMapper mapper): base(repo, mapper)
        {}


        [HttpPut]
        public async Task<IActionResult> UpdateWithName([FromBody] ICollection<VideoUpdate> models)
        {
            if (ModelState.IsValid)
            {
                var update = new List<Video>();
                var videos = await _repo.Item().ToArrayAsync();
                foreach (var video in videos)
                {
                    var model = models.Where(m => m.Name.ToLower() == video.Name.ToLower()).FirstOrDefault();
                    if (model != null)
                    {
                        video.Level = model.Level;
                        update.Add(video);
                    }
                }
                if (update.Count > 0)
                {
                    var (succeeded, updatedModels, error) = _repo.Update(update);
                    if (succeeded) return Ok(updatedModels);
                    return BadRequest(new { Message = error });
                }
                return NoContent();
            }
            return BadRequest(new { Errors = ModelState.Values.SelectMany(e => e.Errors).ToList() });
        }
    }

    public class VideoUpdate
    {
        public string Name { get; set; }
        public Level Level { get; set; }
    }
}
