using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class UserVideo : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DateWatched { get; set; } = DateTime.Now;
        public bool Deleted { get; set; }
        public float Duration { get; set; }

        [ForeignKey("UserTestId")]
        public UserTest UserTest { get; set; }
        public int UserTestId { get; set; }

        [ForeignKey("VideoId")]
        public Video Video { get; set; }
        public int VideoId { get; set; }

    }

    namespace ViewModels
    {
        public class UserVideoViewModel
        {
            public float Duration { get; set; }
            public int UserTestId { get; set; }
            public int VideoId { get; set; }
        }
    }

    namespace DTOs
    {
        public class UserVideoDTO : UserVideo
        { }
    }
}
