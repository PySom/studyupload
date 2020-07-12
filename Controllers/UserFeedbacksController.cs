using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserFeedbacksController : GenericController<UserFeedback, UserFeedbackViewModel, UserFeedbackDTO>
    {
        public UserFeedbacksController(IModelManager<UserFeedback> repo, IMapper mapper): base(repo, mapper)
        {}
    }
}
