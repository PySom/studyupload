using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Award : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int Point { get; set; }

    }

    namespace ViewModels
    {
        public class AwardViewModel : Award
        { }
    }

    namespace DTOs
    {
        public class AwardDTO : Award
        { }
    }
}

