using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class UserSubscription : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public bool Deleted { get; set; }
        public int UserId { get; set; }
        [ForeignKey("SubId")]
        public Subscription Subscription { get; set; }
        public int SubId { get; set; }
        public DateTime StartedOn { get; set; } = DateTime.Now;
    }

    namespace ViewModels
    {
        public class UserSubscriptionVM
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int SubId { get; set; }
        }
    }

    namespace DTOs
    {
        public class UserSubscriptionDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int SubId { get; set; }
            public DateTime StartedOn { get; set; }
        }
    }
}
