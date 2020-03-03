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

        [ForeignKey("PracticeId")]
        public Quiz Practice { get; set; }
        public int PracticeId { get; set; }

    }
}
