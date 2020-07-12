//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace StudyMATEUpload.Models
//{

//    public class UserLearnCourse : IModel
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        public DateTime DateAdded { get; set; } = DateTime.Now;

//        [ForeignKey("UserId")]
//        public ApplicationUser User { get; set; }
//        public int UserId { get; set; }

//        [ForeignKey("LearnCourseId")]
//        public LearnCourse LearnCourse { get; set; }
//        public int LearnCourseId { get; set; }

//        public ICollection<UserVideo> UserVideos { get; set; }
//        public ICollection<UserQuiz> UserQuizzes { get; set; }
//    }

//    namespace ViewModels
//    {
//        public class UserLearnCourseViewModel
//        {
//            public int UserId { get; set; }
//            public int LearnCourseId { get; set; }
//        }
//    }

//    namespace DTOs
//    {
//        public class UserLearnCourseDTO : UserLearnCourse
//        { }
//    }
//}
