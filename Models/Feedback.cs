using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Feedback : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        public bool Deleted { get; set; }
        public int? QUizId { get; set; }
        [ForeignKey("VideoId")]
        public Video Video { get; set; }
        public int? VideoId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
    }

    namespace ViewModels
    {
        public class FeedbackViewModel : Feedback
        { }
    }

    namespace DTOs
    {
        public class FeedbackDTO : Feedback
        { }
    }
}
