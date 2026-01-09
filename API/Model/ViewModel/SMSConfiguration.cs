using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class SMSConfiguration
    {
        public string RequestURL { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int route { get; set; }
        public int flashsms { get; set; }
        public int DCS { get; set; }
        public string channel { get; set; } = string.Empty;
        public string senderid { get; set; } = string.Empty;
        public string API_Key { get; set; } = string.Empty;
        public List<SMSTemplateModel>? Templates { get; set; }
        public List<CommonTextReplaceModel>? CommonTextReplaces { get; set; }
        public bool EnableTempOTP { get; set; }
    }

    public class SMSTemplateModel
    {
        public string Code { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
    }

    public class CommonTextReplaceModel
    {
        public string StatusCode { get; set; } = string.Empty;
        public string ReplaceText { get; set; } = string.Empty;
    }
}
