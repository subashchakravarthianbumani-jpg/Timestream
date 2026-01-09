using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "account_login_log", keyFieldName: "Id")]
    public class AccountLoginLogModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string LoginId { get; set; } = string.Empty;
        [LogField]
        public string Device { get; set; } = string.Empty;
        [LogField]
        public string UserName { get; set; } = string.Empty;
        [LogField]
        public DateTime CreatedDate { get; set; }
    }
}
