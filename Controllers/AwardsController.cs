using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class AwardsController : GenericController<Award, AwardViewModel, AwardDTO>
    {
        public AwardsController(IModelManager<Award> repo, IMapper mapper): base(repo, mapper)
        {}
    }
}
