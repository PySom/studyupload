using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyMATEUpload.Models.ViewModels
{
    public class LeaderDTO
    {
        public int Id { get; set; }
        public string AwardUrl { get; set; }
        public string UserImg { get; set; }
        public string UserName { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
    }
}
