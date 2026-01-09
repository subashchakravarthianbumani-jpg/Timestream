using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class TenderAmountUpdateModel
    {
        public string TenderId { get; set; } = string.Empty;
        public string UpdatedNote { get; set; } = string.Empty;
        public decimal IncreasedAmount { get; set; }

        public string? SavedBy { get; set; } = string.Empty;
        public string? SavedByUserName { get; set; } = string.Empty;
        public DateTime? SavedDate { get; set; }
    }
    public class ResolveAlertModel
    {
        public string Id { get; set; } = string.Empty;
        public string UpdatedNote { get; set; } = string.Empty;

        public string? SavedBy { get; set; } = string.Empty;
        public string? SavedByUserName { get; set; } = string.Empty;
        public DateTime? SavedDate { get; set; }
    }
}
