using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.DTOs;
using StudyMATEUpload.Models.ViewModels;
using AutoMapper;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class FeedbacksController : GenericController<Feedback, FeedbackViewModel, FeedbackDTO>
    {
        public FeedbacksController(IModelManager<Feedback> repo, IMapper mapper): base(repo, mapper)
        {}
    }
}
