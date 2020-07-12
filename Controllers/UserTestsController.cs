using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;
using System.Threading.Tasks;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserTestsController : GenericController<UserTest, UserTestViewModel, UserTestDTO>
    {
        public UserTestsController(IModelManager<UserTest> repo, IMapper mapper): base(repo, mapper)
        {}

    }
}
