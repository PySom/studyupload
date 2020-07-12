using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class UserPreference : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public bool Deleted { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        [ForeignKey("PreferenceId")]
        public Preference Preference { get; set; }
        public int PreferenceId { get; set; }

    }

    namespace ViewModels
    {
        public class UserPreferenceViewModel
        {
            public bool IsEnabled { get; set; }
            public int UserId { get; set; }
            public int PreferenceId { get; set; }
        }
    }

    namespace DTOs
    {
        public class UserPreferenceDTO : UserPreference
        { }
    }
}
