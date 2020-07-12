using StudyMATEUpload.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class UserTest : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsComplete { get; set; }
        public bool Deleted { get; set; }
        public bool TestStarted { get; set; }
        public bool TestEnded { get; set; }
        public bool VideoStarted { get; set; }
        public bool VideoEnded { get; set; }
        public int Score { get; set; }
        public StudyLevel CurrentLevel { get; set; }
        public StudyLevel FinalLevel { get; set; }
        public DateTime DateTaken { get; set; } = DateTime.Now;

        [ForeignKey("UserCourseId")]
        public UserCourse UserCourse { get; set; }
        public int UserCourseId { get; set; }

        [ForeignKey("TestId")]
        public Test Test { get; set; }
        public int TestId { get; set; }

        public ICollection<UserVideo> UserVideos { get; set; }
        public ICollection<UserQuiz> UserQuizzes { get; set; }
    }

    namespace ViewModels
    {
        public class UserTestViewModel
        {
            public bool IsComplete { get; set; }
            public int Score { get; set; }
            public StudyLevel CurrentLevel { get; set; }
            public StudyLevel FinalLevel { get; set; }
            public int UserCourseId { get; set; }
            public int TestId { get; set; }

        }
    }

    namespace DTOs
    {
        public class UserTestDTO : UserTest
        { }
    }
}
