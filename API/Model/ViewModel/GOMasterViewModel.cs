using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class GOMasterViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string GONumber { get; set; } = string.Empty;
        public DateTime GODate { get; set; }
        public decimal GOCost { get; set; }
        public string GOName { get; set; } = string.Empty;
        public string GODepartment { get; set; } = string.Empty;
        public decimal GORevisedAmount { get; set; }
        public decimal GOTotalAmount { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Suffix { get; set; } = string.Empty;
        public int RunningNumber { get; set; }
        public string LocalGONumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public List<TenderMasterViewModel> TenderList { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }
    }
}
