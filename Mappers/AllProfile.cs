using AutoMapper;
using StudyMATEUpload.Models;
using StudyMATEUpload.Models.DTOs;
using StudyMATEUpload.Models.ViewModels;

namespace StudyMATEUpload.Mappers
{
    public class AllProfile : Profile
    {
        public AllProfile()
        {
            CreateMap<ApplicationUser, RegisterViewModel>()
                .ReverseMap();
            CreateMap<ApplicationUser, UserCoursesViewModel>()
                .ReverseMap();
            CreateMap<ApplicationUser, UserCoursesViewModel>()
                .ReverseMap();
            CreateMap<ApplicationUser, UserViewModel>()
                .ReverseMap();
            CreateMap<ApplicationUser, UserDTO>()
                .ReverseMap();
            CreateMap<Award, AwardViewModel>()
                .ReverseMap();
            CreateMap<Award, AwardDTO>()
                .ReverseMap();
            CreateMap<UserAward, UserAwardDTO>()
                .ReverseMap();
            CreateMap<UserAward, UserAwardViewModel>()
                .ReverseMap();
            CreateMap<Course, CourseViewModel>()
                .ReverseMap();
            CreateMap<Course, CourseDTO>()
                .ReverseMap();
            
            CreateMap<Feedback, FeedbackViewModel>()
                .ReverseMap();
            CreateMap<Feedback, FeedbackDTO>()
                .ReverseMap();

            CreateMap<UserCourse, UserCourseViewModel>()
                .ReverseMap();
            CreateMap<UserCourse, UserCourseDTO>()
                .ReverseMap();
            //CreateMap<LearnCourse, LearnCourseViewModel>()
            //    .ReverseMap();
            //CreateMap<LearnCourse, LearnCourseDTO>()
            //    .ReverseMap();
            CreateMap<Option, OptionViewModel>()
                .ReverseMap();
            CreateMap<Option, OptionDTO>()
                .ReverseMap();
            CreateMap<Quiz, QuizViewModel>()
                .ReverseMap();
            CreateMap<Quiz, QuizDTO>()
                .ReverseMap();
            CreateMap<Referral, ReferralViewModel>()
                .ReverseMap();
            CreateMap<Referral, ReferralDTO>()
                .ReverseMap();
            CreateMap<Subscription, SubscriptionViewModel>()
                .ReverseMap();
            CreateMap<Subscription, SubscriptionDTO>()
                .ReverseMap();
            CreateMap<Test, TestViewModel>()
                .ReverseMap();
            CreateMap<Test, TestDTO>()
                .ReverseMap();
            CreateMap<UserCourse, UserCourseDTO>()
                .ReverseMap();
            CreateMap<UserCourse, UserCoursesViewModel>()
                .ReverseMap();
            CreateMap<UserFeedback, UserFeedbackViewModel>()
                .ReverseMap();
            CreateMap<UserFeedback, UserFeedbackDTO>()
                .ReverseMap();
            CreateMap<UserQuiz, UserQuizViewModel>()
                .ReverseMap();
            CreateMap<UserQuiz, UserQuizDTO>()
                .ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionVM>()
                .ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionDTO>()
                .ReverseMap();
            CreateMap<UserVideo, UserVideoViewModel>()
                .ReverseMap();
            CreateMap<UserVideo, UserVideoDTO>()
                .ReverseMap();
            CreateMap<UserTest, UserTestViewModel>()
                .ReverseMap();
            CreateMap<UserTest, UserTestDTO>()
                .ReverseMap();
            CreateMap<Video, VideoViewModel>()
                .ReverseMap();
            CreateMap<Video, VideoDTO>()
                .ReverseMap();
            //CreateMap<UserLearnCourse, UserLearnCourseDTO>()
            //    .ReverseMap();
            //CreateMap<UserLearnCourse, UserLearnCourseViewModel>()
            //    .ReverseMap();
            CreateMap<Preference, PreferenceDTO>()
                .ReverseMap();
            CreateMap<Preference, PreferenceViewModel>()
                .ReverseMap();
            CreateMap<UserPreference, UserPreferenceDTO>()
                .ReverseMap();
            CreateMap<UserPreference, UserPreferenceViewModel>()
                .ReverseMap();
        }
    }
}
