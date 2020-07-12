using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserAwardsController : GenericController<UserAward, UserAwardViewModel, UserAwardDTO>
    {
        public UserAwardsController(IModelManager<UserAward> repo, IMapper mapper): base(repo, mapper)
        {}

        [HttpGet("leaders/{userId:int}")]
        public async ValueTask<IActionResult> Get(int userId, int id=0)
        {
            if(id != 0)
            {

                var models = await _repo.Item()
                    .Where(u => u.AwardId == id)
                    .Include(u => u.Award)
                    .Include(u => u.User)
                    .OrderBy(u => u.IncreasePointBy)
                    .ToListAsync();

                var item = models.Where(m => m.Id == id && userId == m.UserId).FirstOrDefault();
                var prepare = new List<LeaderDTO>();

                foreach (var leader in models.Take(6))
                {
                    var p = models.IndexOf(leader);
                    var prep = new LeaderDTO
                    {
                        Id = leader.Id,
                        AwardUrl = leader.Award.Url,
                        UserImg = leader.User.Image,
                        UserName = leader.User.FirstName + " " + item.User.SurName,
                        Points = leader.Award.Point * item.IncreasePointBy,
                        Position = p
                    };

                    prepare.Add(prep);
                }
                if (item != null)
                {
                    var myPosition = models.IndexOf(item);
                    var isin = prepare.Any(m => m.Id == item.Id);
                    if (!isin)
                    {
                        var transformedItem = new LeaderDTO
                        {
                            Id = item.Id,
                            AwardUrl = item.Award.Url,
                            UserImg = item.User.Image,
                            UserName = item.User.FirstName + " " + item.User.SurName,
                            Points = item.Award.Point * item.IncreasePointBy,
                            Position = myPosition
                        };

                        prepare.RemoveAt(prepare.Count - 1);
                        prepare.Add(transformedItem);
                        prepare.OrderBy(u => u.Points).ToList();
                    }
                }
                else
                {
                    prepare.RemoveAt(prepare.Count - 1);
                }

                
                return Ok(prepare);
                    
            }
            return BadRequest(new { Message = "Supply leader board type" });
        }
    }
}
