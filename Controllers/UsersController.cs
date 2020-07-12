//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using StudyMATEUpload.Models;
//using StudyMATEUpload.Models.ViewModels;
//using StudyMATEUpload.Repository.Extension;
//using StudyMATEUpload.Repository.Generics;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace StudyMATEUpload.Controllers
//{
//    [Route("api/[controller]")]
//    public class UsersController : Controller
//    {
//        private readonly IModelManager<ApplicationUser> _repo;
//        private readonly IMapper _mapper;
//        public UsersController(IModelManager<ApplicationUser> repo, IMapper mapper)
//        {
//            _repo = repo;
//            _mapper = mapper;
//        }

//        [HttpGet]
//        public async ValueTask<IActionResult> Get()
//        {
//            ICollection<ApplicationUser> users = await _repo
//                                                        .Item()
//                                                        .Include(u => u.UserCourses)
//                                                            .ThenInclude(c => c.Test)
//                                                                .ThenInclude(t => t.Course)
//                                                        .ToListAsync();
//            return Ok(users);

//        }


//    }
//}
