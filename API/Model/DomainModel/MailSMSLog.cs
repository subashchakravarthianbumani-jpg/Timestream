using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    public class MailSMSLog
    {
        public string Id { get; set; } = string.Empty;
        public string RecordType { get; set; } = string.Empty;
        public string SentAddress { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;
        public string ReceivedBy { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;


        public DateTime? CreatedDate { get; set; }
    }
}
