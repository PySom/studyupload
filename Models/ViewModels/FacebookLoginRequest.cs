
using StudyMATEUpload.Enums;

namespace StudyMATEUpload.Models.ViewModels
{
    public class FacebookLoginRequest
    {
        public string ProviderKey { get; set; }
        public string Provider { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
        public Role Role { get; set; }
    }
}
