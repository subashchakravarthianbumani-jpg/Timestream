using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "comment_master", keyFieldName: "Id")]
    public class CommentMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string Type { get; set; } = string.Empty;
        [LogField]
        public string TypeId { get; set; } = string.Empty;
        [LogField]
        public string ParentId { get; set; } = string.Empty;
        [LogField]
        public string CommentsFrom { get; set; } = string.Empty;
        [LogField]
        public string CommentsText { get; set; } = string.Empty;
        [LogField]
        public string SubjectText { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string CommentNumber { get; set; } = string.Empty;
        public DateTime? CommentDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
