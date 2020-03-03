using AutoMapper;
using StudyMATEUpload.Models;
using StudyMATEUpload.Models.ViewModels;

namespace StudyMATEUpload.Mappers
{
    public class AllProfile : Profile
    {
        public AllProfile()
        {
            CreateMap<ApplicationUser, RegisterViewModel>()
                .ReverseMap();
            CreateMap<Course, CourseViewModel>()
                .ReverseMap();
            CreateMap<Course, Course>()
                .ForAllMembers(c => c.Condition((src,dest,srcMember,destMember) => srcMember != null && srcMember != destMember));
        }
    }
}
