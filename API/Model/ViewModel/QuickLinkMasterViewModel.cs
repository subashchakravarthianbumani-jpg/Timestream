using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class QuickLinkMasterViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string UserGroupIds { get; set; } = string.Empty;
        public List<string>? UserGroupIdList { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }
    }
}
