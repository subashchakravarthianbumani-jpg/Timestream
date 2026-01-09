using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class MbookApprovalModel
    {
        public string MbookId { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string MbookApprovHistoryeId { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public string SavedByRoleName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
