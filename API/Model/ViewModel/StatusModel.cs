using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class StatusModel
    {
        public string Id { get; set; }=string.Empty;
        public string WorkNumber { get; set; }=string.Empty;
        public int row_num { get; set; }
        public decimal TenderAmount { get; set;}
        public string Status { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string ContractorId { get; set; } = string.Empty;



    }
}
