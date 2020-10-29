using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    
    public class Referral : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Link { get; set; } = Guid.NewGuid().ToString();
        public DateTime DateIssued { get; set; } = DateTime.Now;
        public int Reach { get; set; }
        public bool Deleted { get; set; }
        public int SignUps { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        public ICollection<ApplicationUser> Referrals { get; set; }

    }

    namespace ViewModels
    {
        public class ReferralViewModel
        {
            public int UserId { get; set; }
        }
    }

    namespace DTOs
    {
        public class ReferralDTO : Referral
        { }
    }
}
