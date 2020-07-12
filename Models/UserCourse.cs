using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class UserCourse : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateTaken { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public int CourseId { get; set; }

        public ICollection<UserTest> UserTests { get; set; }
    }

    namespace ViewModels
    {
        public class UserCourseViewModel
        {
            public int UserId { get; set; }
            public int CourseId { get; set; }
        }
    }

    namespace DTOs
    {
        public class UserCourseDTO : UserCourse
        { }
    }
}
