using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.UtilModels
{
    public class SMSModel
    {
        public List<string> MobileNumbers { get; set; } = new List<string>();
        public string TemplateCode { get; set; } = string.Empty;
        public IDictionary<string, string> MessageReplaces { get; set; } = new Dictionary<string, string>();

        // Application
        public string Type { get; set; } = string.Empty;
        public string TypeId { get; set; } = string.Empty;
        public string ReceivedBy { get; set; } = string.Empty;
        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
