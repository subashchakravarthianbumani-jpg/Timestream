using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CustomeAttributes;

namespace Model.DomainModel
{
    public class ConfigCategoryModel
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

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime ModifiedDate { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public string DeletedByUserName { get; set; } = string.Empty;
        public DateTime DeletedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }

    [TableInfo(tableName: "two_column_configuration_values", keyFieldName: "Id")]
    public class ConfigurationModel
    {
        [LogField]
        public string Id { get; set; } = null!;
        [LogField]
        public string CategoryId { get; set; } = null!;
        [LogField]
        public string ConfigurationId { get; set; } = null!;
        [LogField]
        public string Value { get; set; } = null!;
        [LogField]
        public string? Code { get; set; } = string.Empty;
        [LogField]
        public string? DepartmentId { get; set; } = string.Empty;
        [LogField]
        public string? DepartmentName { get; set; } = string.Empty;
        [LogField]
        public string? DepartmentCode { get; set; } = string.Empty;
        [LogField]
        public bool CanDelete { get; set; }
        [LogField]
        public bool IsGeneral { get; set; }
        [LogField]
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByUserName { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public string DeletedByUserName { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }


        public string SchemeCode { get; set; } = string.Empty;
        public string SchemeId { get; set; } = string.Empty;

        public string SchemeName { get; set; } = string.Empty;
        public long Count { get; set; }

    }


    public class RecordingRequest
    {
        public string CameraId { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }




}
