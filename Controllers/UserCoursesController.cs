using Microsoft.AspNetCore.Mvc;
using StudyMATEUpload.Models;
using StudyMATEUpload.Repository.Generics;
using StudyMATEUpload.Models.ViewModels;
using StudyMATEUpload.Models.DTOs;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using StudyMATEUpload.Enums;
using System.Collections.Generic;
using System;

namespace StudyMATEUpload.Controllers
{
    [Route("api/[controller]")]
    public class UserCoursesController : GenericController<UserCourse, UserCourseViewModel, UserCourseDTO>
    {
        private readonly IModelManager<Test> _test;
        public UserCoursesController(IModelManager<UserCourse> repo, IMapper mapper, IModelManager<Test> test) : base(repo, mapper)
        {
            _test = test;
        }

        [HttpGet("withparams/{courseId:int}/user/{userId:int}")]
        public async ValueTask<IActionResult> GetUserCoursesWithExtras(int courseId, int userId)
        {
            return Ok(await _repo.Item()
                        .Where(u => u.CourseId == courseId && u.UserId == userId)
                        .Include(u => u.Course)
                        .Include(u => u.UserTests)
                            .ThenInclude(u => u.UserQuizzes)
                        .Include(u => u.UserTests)
                            .ThenInclude(u => u.UserVideos)
                        .FirstOrDefaultAsync()
                    );
        }

        [HttpGet("all/{userId:int}")]
        public async ValueTask<IActionResult> GetAllUserCourses(int userId)
        {
            return Ok(await _repo.Item()
                        .Where(u => u.UserId == userId)
                        .Include(u => u.Course)
                        .Include(u => u.UserTests)
                            .ThenInclude(u => u.UserQuizzes)
                        .Include(u => u.UserTests)
                            .ThenInclude(u => u.UserVideos)
                        .FirstOrDefaultAsync()
                    );
        }

        [HttpGet("withusercourse/{id:int}")]
        public async ValueTask<IActionResult> GetUserCoursesWithExtras(int id)
        {
            return Ok(await _repo.Item()
                        .Where(u => u.Id == id)
                        .Include(u => u.Course)
                        .Include(u => u.UserTests)
                            .ThenInclude(u => u.UserQuizzes)
                        .Include(u => u.UserTests)
                            .ThenInclude(u => u.UserVideos)
                        .FirstOrDefaultAsync()
                    );
        }

        [HttpPost]
        public override async ValueTask<IActionResult> Post([FromBody] UserCourseViewModel model)
        {
            var courseAdded = await _repo.Item().FirstOrDefaultAsync(m => m.CourseId == model.CourseId && m.UserId == model.UserId);
            if (courseAdded is object)
            {
                if(courseAdded.Deleted)
                {
                    courseAdded.Deleted = false;
                    return await Put(courseAdded);
                }
                return BadRequest(new { Message = "You already have this to your list of courses" });
            }
            return await base.Post(model);
        }

        [HttpDelete("course/{cid:int}/user/{uid:int}")]
        public async ValueTask<IActionResult> DeleteWithCourse(int cid, int uid)
        {
            UserCourse model = await _repo.FindOne(uc => uc.CourseId == cid && uc.UserId == uid);
            return await Hide(model);
        }


        [HttpGet("statistics/{id:int}")]
        public async ValueTask<IActionResult> GetUserStats(int id)
        {
            var data = await _repo.Item().Where(uc => uc.Id == id)
                                            .Include(uc => uc.UserTests)
                                                .ThenInclude(ut => ut.UserQuizzes)
                                            .Include(uc => uc.UserTests)
                                                .ThenInclude(ut => ut.UserVideos)
                                            .Include(uc => uc.UserTests)
                                                .ThenInclude(ut => ut.Test)
                                            .FirstOrDefaultAsync();

            var tests = await _test.Item().Where(c => c.CourseId == data.CourseId)
                .Include(c => c.Videos)
                .Include(c => c.Quizes)
                .Select(c => new { c.Id, Quizzes = c.Quizes.Count, Videos = c.Videos.Count })
                .ToListAsync();
            float allQuizzes, allVideos, userQuizzes, userVideos, possiblePoints, userPoint, percentQuiz, percentVideo, percentMastery = 0;
            var activities = new List<Activity>();
            if (data != null)
            {
                allQuizzes = tests.Aggregate(0, (aggregate, current) =>
                {
                    aggregate += current.Quizzes;
                    return aggregate;
                });

                allVideos = tests.Aggregate(0, (aggregate, current) =>
                {
                    aggregate += current.Videos;
                    return aggregate;
                });

                userQuizzes = data.UserTests.Aggregate(0, (aggregate, current) =>
                {
                    aggregate += current.UserQuizzes.Where(u => u.Mode == Mode.Free).Count();
                    return aggregate;
                });

                userVideos = data.UserTests.Aggregate(0, (aggregate, current) =>
                {
                    aggregate += current.UserVideos.Count();
                    return aggregate;
                });

                possiblePoints = allQuizzes * 2;
                userPoint = userQuizzes * 2;

                percentQuiz = allQuizzes == 0 ? 0 : userQuizzes / allQuizzes;
                percentVideo = allVideos == 0 ? 0 : userVideos / allVideos;
                percentMastery = possiblePoints == 0 ? 0 : userPoint / possiblePoints;

                var ut = new List<UserQuizClassification>();
                var uv = new List<UserVideoClassification>();


                foreach (var userTest in data.UserTests)
                {
                    //get for ten levels for video and quizzes
                    ut.AddRange(userTest.UserQuizzes.Aggregate(new List<UserQuizClassification>(), (aggregate, current) =>
                    {
                        var agg = aggregate.Find(c => 
                        c.UserTestId == current.UserTestId 
                        && c.DateTime.Date == current.DateTaken.Date);
                        if (agg != null)
                        {
                            aggregate.Remove(agg);
                            agg.UserQuizzes.Add(current);
                            aggregate.Add(agg);
                        }
                        else
                        {
                            var ags = new UserQuizClassification 
                            { 
                                UserTestId = current.UserTestId,
                                TestId = current.UserTest.TestId,
                                DateTime = current.DateTaken.Date, 
                                UserQuizzes = new List<UserQuiz> { current } 
                            };
                            aggregate.Add(ags);
                        }
                        return aggregate;
                    }));

                    uv.AddRange(userTest.UserVideos.Aggregate(new List<UserVideoClassification>(), (aggregate, current) =>
                    {
                        var agg = aggregate.Find(c =>
                        c.UserTestId == current.UserTestId
                        && c.DateTime.Date == current.DateWatched.Date);
                        if (agg != null)
                        {
                            aggregate.Remove(agg);
                            agg.UserVideo.Add(current);
                            aggregate.Add(agg);
                        }
                        else
                        {
                            var ags = new UserVideoClassification
                            {
                                UserTestId = current.UserTestId,
                                TestId = current.UserTest.TestId,
                                DateTime = current.DateWatched.Date,
                                UserVideo = new List<UserVideo> { current }
                            };
                            aggregate.Add(ags);
                        }
                        return aggregate;
                    }));

                }


                if (ut.Count > 0)
                {
                    foreach (var item in ut)
                    {
                        var totalQuizin = 0;
                        var point = item.UserQuizzes.Aggregate(0, (aggregate, current) =>
                        {
                            aggregate += current.CorrectOption == current.UserOption ? 1 : 0;
                            return aggregate;
                        });
                        var testQuiz = tests.Where(u => u.Id == item.TestId).FirstOrDefault();
                        if (testQuiz != null)
                        {
                            totalQuizin = testQuiz.Quizzes;
                        }
                        activities.Add(new Activity
                        {
                            Description = "Quiz",
                            Date = item.DateTime.ToShortDateString(),
                            TestType = "Quiz",
                            Point = point,
                            Overall = $"{point}/{totalQuizin}",
                            DayOfWeek = item.DateTime.DayOfWeek
                        });
                    }
                }

                if (uv.Count > 0)
                {
                    foreach (var item in uv)
                    {
                        var totalVideoin = 0;
                        var point = item.UserVideo.Count();
                        var testQuiz = tests.Where(u => u.Id == item.TestId).FirstOrDefault();
                        if (testQuiz != null)
                        {
                            totalVideoin = testQuiz.Videos;
                        }

                        point = point > totalVideoin ? totalVideoin : point;
                        activities.Add(new Activity
                        {
                            Description = "Video",
                            Date = item.DateTime.ToShortDateString(),
                            TestType = "Video",
                            Point = 0,
                            Overall = $"{point}/{totalVideoin}",
                            DayOfWeek = item.DateTime.DayOfWeek
                        });
                    }
                }

                var dto = new Statistics
                {
                    PercentQuiz = String.Format("{0:0.##}", percentQuiz),
                    PercentVideo = String.Format("{0:0.##}", percentVideo),
                    PercentMastery = String.Format("{0:0.##}", percentMastery),
                    Activities = activities
                };

                return Ok(dto);
            }
            return BadRequest(new { Message = "No such item" });
        }

        class Statistics
        {
            public string PercentQuiz { get; set; }
            public string PercentVideo { get; set; }
            public string PercentMastery { get; set; }
            public List<Activity> Activities { get; set; }
        }

        class UserQuizClassification
        {
            public Level Level { get; set; }
            public int UserTestId { get; set; }
            public int TestId { get; set; }
            public DateTime DateTime { get; set; }
            public List<UserQuiz> UserQuizzes { get; set; }
        }

        class UserVideoClassification
        {
            public Level Level { get; set; }
            public int UserTestId { get; set; }
            public int TestId { get; set; }
            public DateTime DateTime { get; set; }
            public List<UserVideo> UserVideo { get; set; }
        }

        class Activity
        {
            public string Description { get; set; }
            public string Date { get; set; }
            public Level Level { get; set; }
            public int Point { get; set; }
            public string TestType { get; set; }
            public DayOfWeek DayOfWeek { get; set; }
            public string Overall { get; set; }
        }
    }
}
