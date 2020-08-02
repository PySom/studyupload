using StudyMATEUpload.Enums;
using System;

namespace StudyMATEUpload.Models.ViewModels
{
    public class UserViewModel
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
        public Role Role { get; set; }
        public bool IsVerified { get; set; }
        public bool IsSubscribed { get; set; }
        public string Token { get; set; }
    }

    public class UserCoursesViewModel
    {
        public string SurName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public float TestScore { get; set; }
        public float Score { get; set; }
        public string Sex { get; set; }
        public DateTime DateTaken { get; set; }
        public int TestId { get; set; }
    }
}
