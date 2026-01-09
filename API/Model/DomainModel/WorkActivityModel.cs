using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "work_activity", keyFieldName: "Id")]
    public class WorkActivityModel
    {
        [LogField]
        public string Id { get; set; } = null!;
        [LogField]
        public string Type { get; set; } = null!;
        [LogField]
        public string TypeId { get; set; } = null!;
        [LogField]
        public string ParentType { get; set; } = null!;
        [LogField]
        public string ParentId { get; set; } = null!;
        [LogField]
        public string ActivitySubject { get; set; } = null!;
        [LogField]
        public string ActivityMessage { get; set; } = null!;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
