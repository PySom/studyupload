using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class UserFeedback : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool Deleted { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        [ForeignKey("FeedbackId")]
        public Feedback Feedback { get; set; }
        public int FeedbackId { get; set; }

    }

    namespace ViewModels
    {
        public class UserFeedbackViewModel
        {
            public int UserId { get; set; }
            public int FeedbackId { get; set; }
        }
    }

    namespace DTOs
    {
        public class UserFeedbackDTO : UserFeedback
        { }
    }
}
