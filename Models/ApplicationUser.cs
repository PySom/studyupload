using StudyMATEUpload.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyMATEUpload.Models
{
    public class ApplicationUser : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SurName { get; set; }
        public string FirstName { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public string UserName { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public StudyLevel Level { get; set; }
        public Sex Sex { get; set; }
        public string Image { get; set; }
        public float TestScore { get; set; }
        public bool IsVerified { get; set; }
        [NotMapped]
        public bool IsSubscribed { get; set; }
        public bool Deleted { get; set; }
        public DateTime VerifiedOn { get; set; }
        public Role Role { get; set; }
        public string PasswordHash { get; set; }
        public string Code { get; set; }
        public DateTime CodeIssued { get; set; }
        public DateTime CodeWillExpire { get; set; }
        public ICollection<UserCourse> UserCourses { get; set; }
        public ICollection<Feedback> UserFeedbacks { get; set; }
        public ICollection<UserSubscription> UserSubscriptions { get; set; }
        public ICollection<UserAward> UserAwards { get; set; }
    }

    namespace DTOs
    {
        public class UserDTO
        {
            public int Id { get; set; }
            public string SurName { get; set; }
            public string FirstName { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime DOB { get; set; }
            public string UserName { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string Location { get; set; }
            public string Email { get; set; }
            public Sex Sex { get; set; }
            public StudyLevel Level { get; set; }
            public string Image { get; set; }
            public bool IsVerified { get; set; }
            public Role Role { get; set; }
            public ICollection<UserCourse> UserCourses { get; set; }
            public ICollection<UserFeedback> UserFeedbacks { get; set; }
            public ICollection<UserSubscription> UserSubscriptions { get; set; }
            public ICollection<UserAward> UserAwards { get; set; }
        }
    }
}
