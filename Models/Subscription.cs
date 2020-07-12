using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class Subscription : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string Description { get; set; }
        public int TotalAllowable { get; set; }
        public byte Percentage { get; set; }
        public TimeSpan Duration { get; set; }
    }

    namespace ViewModels
    {
        public class SubscriptionViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int TotalAllowable { get; set; }
            public byte Percentage { get; set; }
            public TimeSpan Duration { get; set; }
        }
    }

    namespace DTOs
    {
        public class SubscriptionDTO : Subscription
        { }
    }
}
