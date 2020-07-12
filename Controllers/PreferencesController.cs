using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class PreferencesController : GenericController<Preference, PreferenceViewModel, PreferenceDTO>
    {
        public PreferencesController(IModelManager<Preference> repo, IMapper mapper): base(repo, mapper)
        {}
    }
}
