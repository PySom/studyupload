//using StudyMATEUpload.Enums;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace StudyMATEUpload.Models
//{
//    public class LearnCourse : IModel
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int Id { get; set; }
//        public StudyLevel StudyLevel { get; set; }
//        public string Name { get; set; }
//        public string Alias { get; set; }
//        public string SubText { get; set; }

//        public ICollection<Quiz> Quizzes { get; set; }
//        public ICollection<Video> Videos { get; set; }
//    }
//    namespace ViewModels
//    {
//        public class LearnCourseViewModel
//        {
//            public StudyLevel StudyLevel { get; set; }
//            public string Alias { get; set; }
//            public string Name { get; set; }
//            public string SubText { get; set; }

//        }
//    }

//    namespace DTOs
//    {
//        public class LearnCourseDTO : LearnCourse
//        { }
//    }
//}
