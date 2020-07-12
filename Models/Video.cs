using StudyMATEUpload.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Video : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Level Level { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public float Duration { get; set; }

        [ForeignKey("TestId")]
        public Test Test { get; set; }
        public int TestId { get; set; }

    }

    namespace ViewModels
    {
        public class VideoViewModel
        {
            public Level Level { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public float Duration { get; set; }
            public int TestId { get; set; }
        }
    }

    namespace DTOs
    {
        public class VideoDTO : Video
        { }
    }
}
