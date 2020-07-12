using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserSubscriptionsController : GenericController<UserSubscription, UserSubscriptionVM, UserSubscriptionDTO>
    {
        public UserSubscriptionsController(IModelManager<UserSubscription> repo, IMapper mapper): base(repo, mapper)
        {}
    }
}
