using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudyMATEUpload.Enums;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserQuizzesController : GenericController<UserQuiz, UserQuizViewModel, UserQuizDTO>
    {
        public UserQuizzesController(IModelManager<UserQuiz> repo, IMapper mapper): base(repo, mapper)
        {}

        //[HttpGet("summary/{userCourseId:int}")]
        //public async ValueTask<IActionResult> GetSummary(int userCourseId)
        //{
        //    int userScore = 0;
        //    var userCourses = await _repo.Item()
        //        .Where(u => u.UserCourseId == userCourseId)
        //        .ToListAsync();
        //    if(userCourses.Count > 0)
        //    {
        //        userScore = userCourses.Aggregate(0, (agr, next) => (next.CorrectOption == next.UserOption) ? agr + 1 : agr);
        //    }
        //    return Ok(new { Score = userScore });
        //}

        //[HttpGet("summary/{userCourseId:int}/level/{level}")]
        //public async ValueTask<IActionResult> GetSummary(int userCourseId, Level level)
        //{
        //    int userScore = 0;
        //    var userCourses = await _repo.Item()
        //        .Where(u => u.UserCourseId == userCourseId && u.Quiz.Level == level)
        //        .ToListAsync();
        //    if (userCourses.Count > 0)
        //    {
        //        userScore = userCourses.Aggregate(0, (agr, next) => (next.CorrectOption == next.UserOption) ? agr + 1 : agr);
        //    }
        //    return Ok(new { Score = userScore });
        //}

        //[HttpGet("coursesummary/{id:int}")]
        //public async ValueTask<IActionResult> GetAllCourseScore(int id)
        //{
        //    int userScore = 0;
        //    var userCourses = await _repo.Item()
        //        .Where(u => u.UserCourseId == id)
        //        .ToListAsync();
        //    if (userCourses.Count > 0)
        //    {
        //        userScore = userCourses.Aggregate(0, (agr, next) => (next.CorrectOption == next.UserOption) ? agr + 1 : agr);
        //    }
        //    return Ok(new { Score = userScore });
        //}

        //[HttpGet("learncoursesummary/{id:int}")]
        //public async ValueTask<IActionResult> GetAllLearnCourseScore(int id)
        //{
        //    int userScore = 0;
        //    var userCourses = await _repo.Item()
        //        .Where(u => u.UserLearnCourseId == id)
        //        .ToListAsync();
        //    if (userCourses.Count > 0)
        //    {
        //        userScore = userCourses.Aggregate(0, (agr, next) => (next.CorrectOption == next.UserOption) ? agr + 1 : agr);
        //    }
        //    return Ok(new { Score = userScore });
        //}

        

    }
}
