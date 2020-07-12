using StudyMATEUpload.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{

    public class UserQuiz : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Mode Mode { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateTaken { get; set; } = DateTime.Now;
        public int CorrectOption { get; set; }
        public int UserOption { get; set; }

        [ForeignKey("UserTestId")]
        public UserTest UserTest { get; set; }
        public int UserTestId { get; set; }


        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        public int QuizId { get; set; }

    }

    namespace ViewModels
    {
        public class UserQuizViewModel
        {
            public int CorrectOption { get; set; }
            public int UserOption { get; set; }
            public int UserTestId { get; set; }
            public int QuizId { get; set; }
            public Mode Mode { get; set; }
        }
    }

    namespace DTOs
    {
        public class UserQuizDTO : UserQuiz
        { }
    }
}
