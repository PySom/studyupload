using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class UserAward : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IncreasePointBy { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateAwarded { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        [ForeignKey("AwardId")]
        public Award Award { get; set; }
        public int AwardId { get; set; }

    }

    namespace ViewModels
    {
        public class UserAwardViewModel
        {
            public int UserId { get; set; }
            public int AwardId { get; set; }
        }
    }

    namespace DTOs
    {
        public class UserAwardDTO : UserAward
        { }
    }
}
