using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.UtilModels;

namespace Utils.Interface
{
    public interface IMailHelper
    {
        public bool SendMail(EmailModel config, out string body, out string subject);
        public void SendMailAsync(EmailModel config);
    }
}
