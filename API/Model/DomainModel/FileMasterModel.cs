using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "file_master", keyFieldName: "Id")]
    public class FileMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string Type { get; set; } = string.Empty;
        [LogField]
        public string OriginalFileName { get; set; } = string.Empty;
        [LogField]
        public string SavedFileName { get; set; } = string.Empty;
        [LogField]
        public string FileType { get; set; } = string.Empty;
        [LogField]
        public string TypeId { get; set; } = string.Empty;
        [LogField]
        public string TypeName { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedbyRole { get; set; } = string.Empty;
        public string Createdbydivion { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public string DeletedByUserName { get; set; } = string.Empty;
        public DateTime DeletedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
