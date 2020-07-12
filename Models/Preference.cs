using StudyMATEUpload.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Preference : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    namespace ViewModels
    {
        public class PreferenceViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }

    namespace DTOs
    {
        public class PreferenceDTO : Preference
        { }
    }
}
