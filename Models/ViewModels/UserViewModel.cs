namespace StudyMATEUpload.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string SurName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public bool IsAdmin { get; set; }
        public string Token { get; set; }
    }
}
