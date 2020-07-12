
using StudyMATEUpload.Enums;

namespace StudyMATEUpload.Models.ViewModels
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
        public Role Role { get; set; }
    }
}
