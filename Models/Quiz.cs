using StudyMATEUpload.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Quiz : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IncludeThis { get; set; }
        public bool IsSectioned { get; set; }
        public bool IsFirstSection { get; set; }
        public Level Level { get; set; }
        public bool Deleted { get; set; }
        public int QuestionNumber { get; set; }
        public string Section { get; set; }
        public bool HasPassage { get; set; }
        public bool IsFirstPassage { get; set; }
        public string Passage { get; set; }
        public string Question { get; set; }
        public bool IsQuestionMathJax { get; set; }
        public int AnswerId { get; set; }
        public bool HasVideo { get; set; }
        public string VideoUrl { get; set; }
        public string AudioUrl { get; set; }
        public string AnswerUrl { get; set; }

        [ForeignKey("TestId")]
        public Test Test { get; set; }
        public int TestId { get; set; }

        public ICollection<Option> Options { get; set; }

    }

    namespace ViewModels
    {
        public class QuizViewModel
        {
            public bool IsSectioned { get; set; }
            public bool IncludeThis { get; set; }
            public bool IsFirstSection { get; set; }
            public Level Level { get; set; }
            public int QuestionNumber { get; set; }
            public string Section { get; set; }
            public bool HasPassage { get; set; }
            public bool IsFirstPassage { get; set; }
            public string Passage { get; set; }
            public string Question { get; set; }
            public bool IsQuestionMathJax { get; set; }
            public int AnswerId { get; set; }
            public bool HasVideo { get; set; }
            public string VideoUrl { get; set; }
            public string AudioUrl { get; set; }
            public string AnswerUrl { get; set; }
            public int? TestId { get; set; }
            public int? LearnCourseId { get; set; }
        }
    }

    namespace DTOs
    {
        public class QuizDTO : Quiz
        { }
    }
}
