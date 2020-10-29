using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.DTOs;
using StudyMATEUpload.Models.ViewModels;
using AutoMapper;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class ReferralsController : GenericController<Referral, ReferralViewModel, ReferralDTO>
    {
        public ReferralsController(IModelManager<Referral> repo, IMapper mapper): base(repo, mapper)
        {}

        [HttpGet("link/{link}")]
        public async ValueTask<IActionResult> GetReferralId(string link)
        {
            var id = await _repo.Item().Where(r => r.Link == link).Select(r => r.Id).FirstOrDefaultAsync();
            if(id != 0)
            {
                return Ok(new { id });
            }
            return NotFound();
        }
    }
}
