using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ConfigurationViewModel
    {
        public string Id { get; set; } = null!;
        public string CategoryId { get; set; } = null!;
        public string ConfigurationId { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string? Code { get; set; } = string.Empty;
        public string? DepartmentId { get; set; } = string.Empty;
        public string? DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; } = string.Empty;
        public bool CanDelete { get; set; }
        public bool IsGeneral { get; set; }
        public bool IsActive { get; set; }

        public string LastUpdatedBy { get; set; } = string.Empty;
        public string LastUpdatedUserName { get; set; } = string.Empty;
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class ConfigCategoryViewModel
    {
        public string Id { get; set; } = null!;
        public string ParentId { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string CategoryCode { get; set; } = null!;
        public string CategoryType { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDependent { get; set; }
        public bool HasCode { get; set; }
    }
}
