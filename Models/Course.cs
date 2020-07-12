using StudyMATEUpload.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Course : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public StudyLevel StudyLevel { get; set; }
        public bool Deleted { get; set; }
        public bool HasStudyPack { get; set; }
        public bool IsVisible { get; set; }
        public bool HasMain { get; set; }
        public string Name { get; set; }
        public string SubText { get; set; }

        public ICollection<Test> Tests { get; set; }
    }

    namespace ViewModels
    {
        public class CourseViewModel : Course
        { }
    }

    namespace DTOs
    {
        public class CourseDTO : Course
        { }
    }
}

