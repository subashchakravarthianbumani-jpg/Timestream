using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class FileUploadFormModel
    {
        public string Type { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

    public class UserProfileUploadFormModel
    {
        public string UserId { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

    public class WorkFileUploadFormModel
    {
        public string Type { get; set; } = string.Empty;
        public string WorkId { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }

    public class MilestoneFileUploadFormModel
    {
        public string MilestoneId { get; set; } = string.Empty;
        public List<IFormFile> Files { get; set; } = null!;
    }
}
