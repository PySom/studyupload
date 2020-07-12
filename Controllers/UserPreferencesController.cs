using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserPreferencesController : GenericController<UserPreference, UserPreferenceViewModel, UserPreferenceDTO>
    {
        public UserPreferencesController(IModelManager<UserPreference> repo, IMapper mapper): base(repo, mapper)
        {}
    }
}
