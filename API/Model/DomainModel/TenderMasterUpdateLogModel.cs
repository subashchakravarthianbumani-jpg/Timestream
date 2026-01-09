using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    public class TenderMasterUpdateLogModel
    {
        public string Id { get; set; } = string.Empty;
        public int AddedRecordCount { get; set; }
        public string ResponceText { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
