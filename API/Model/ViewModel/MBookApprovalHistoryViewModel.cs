using Model.CustomeAttributes;
using Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class MBookApprovalHistoryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string MBookId { get; set; } = string.Empty;
        public string FromId { get; set; } = string.Empty;
        public string ToId { get; set; } = string.Empty;
        public string StatusEnum { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string FromRoleName { get; set; } = string.Empty;
        public string ToRoleName { get; set; } = string.Empty;

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }
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
