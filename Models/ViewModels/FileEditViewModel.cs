using System.ComponentModel.DataAnnotations;

namespace StudyMATEUpload.Models.ViewModels
{
    public class FileEditViewModel : FileViewModel
    {
        [Required]
        public string OldImage { get; set; }
    }
}
