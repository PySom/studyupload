using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Option : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Content { get; set; }
        public bool IsMathJax { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("PracticeId")]
        public Quiz Practice { get; set; }
        public int PracticeId { get; set; }

    }
    namespace ViewModels
    {
        public class OptionViewModel
        {
            public string Content { get; set; }
            public bool IsMathJax { get; set; }
            public int PracticeId { get; set; }
        }
    }

    namespace DTOs
    {
        public class OptionDTO : Option
        { }
    }

}
