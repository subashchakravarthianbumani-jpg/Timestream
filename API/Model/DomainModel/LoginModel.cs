using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "account_login", keyFieldName: "LoginId")]
    public class LoginModel
    {
        [LogField]
        public string LoginId { get; set; } = null!;
        [LogField]
        public string UserId { get; set; } = null!;
        [LogField]
        public string UserName { get; set; } = string.Empty;
        [LogField]
        public string Password { get; set; } = string.Empty;
        [LogField]
        public string Email { get; set; } = string.Empty;
        [LogField]
        public string AccessToken { get; set; } = string.Empty;
        [LogField]
        public string RefreshToken { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        [LogField]
        public DateTime LastLoginDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
