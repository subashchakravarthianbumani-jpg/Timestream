using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "quicklink_master", keyFieldName: "Id")]
    public class QuickLinkMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string Name { get; set; } = string.Empty;
        [LogField]
        public string Link { get; set; } = string.Empty;
        [LogField]
        public string FileType { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        [LogField]
        public string UserGroupIds { get; set; } = string.Empty;
        [LogField]
        public List<string>? UserGroupIdList { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public string DeletedByUserName { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
