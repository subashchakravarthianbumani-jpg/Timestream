using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "work_mbook_approval_history", keyFieldName: "Id")]
    public class MBookApprovalHistoryModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string MBookId { get; set; } = string.Empty;
        [LogField]
        public string FromId { get; set; } = string.Empty;
        [LogField]
        public string ToId { get; set; } = string.Empty;
        [LogField]
        public string StatusEnum { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        [LogField]
        public string Comments { get; set; } = string.Empty;
        [LogField]
        public string FromRoleName { get; set; } = string.Empty;
        [LogField]
        public string ToRoleName { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<FileMasterModel>? Files { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;

        public string SavedFileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public string TypeId { get; set; } = string.Empty;

        public string TypeName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

    }
}
