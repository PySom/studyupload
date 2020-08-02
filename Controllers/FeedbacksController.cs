using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.DTOs;
using StudyMATEUpload.Models.ViewModels;
using AutoMapper;
using System.Threading.Tasks;
using StudyMATEUpload.Services;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class FeedbacksController : GenericController<Feedback, FeedbackViewModel, FeedbackDTO>
    {
        private readonly IEmailSender _email;
        public FeedbacksController(IModelManager<Feedback> repo, IMapper mapper, IEmailSender email): base(repo, mapper)
        {
            _email = email;
        }

        [HttpPost]
        public override async ValueTask<IActionResult> Post([FromBody] FeedbackViewModel model)
        {
            try
            {
                string message = $"<b>Subject:</b> <br />{model.Subject}<br /><b>Message:</b> <br />{model.Message}<br /><b>User ID:</b> {model.UserId}";
                await _email.SendEmailAsync("contact@infomall.ng", "Feedback", message);
            }
            catch { }
            return await base.Post(model);

        }
    }
}
