using Model.CustomeAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    [TableInfo(tableName: "work_template_master", keyFieldName: "Id")]
    public class WorkTemplateMasterModel
    {
        [LogField]
        public string Id { get; set; } = string.Empty;
        [LogField]
        public string TemplateId { get; set; } = string.Empty;
        [LogField]
        public string WorkId { get; set; } = string.Empty;
        [LogField]
        public string WorkTemplateName { get; set; } = string.Empty;
        [LogField]
        public string WorkTypeId { get; set; } = string.Empty;
        [LogField]
        public int WorkDurationInDays { get; set; }
        [LogField]
        public string WorkTemplateNumber { get; set; } = string.Empty;
        [LogField]
        public string WorkType { get; set; } = string.Empty;
        [LogField]
        public bool IsActive { get; set; }
        [LogField]
        public bool IsSubmitted { get; set; }

        public string SubWorkTypeId { get; set; } = string.Empty;
        public string Strength { get; set; } = string.Empty;
        public string TemplateCode { get; set; } = string.Empty;
        public string SubWorkType { get; set; } = string.Empty;

        public string categoryTypeId { get; set; } = string.Empty;
        public string serviceTypeId { get; set; } = string.Empty;
        public string serviceType { get; set; } = string.Empty;
        public string categoryType { get; set; } = string.Empty;

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
    }


    public class CameraStreamModel
    {
        public string Id { get; set; } = string.Empty ;
        public string IpAddress { get; set; } = string.Empty;

        public string UserName {  get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Channel { get; set; } = string.Empty;

        public string SubType { get; set; } = string.Empty;

        public string WorkId { get; set; } = string.Empty;

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }

      public class RtspUrlModel
    {
        public string Id { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Channel { get; set; } = string.Empty;

        public string SubType { get; set; } = string.Empty;

        public string WorkId { get; set; } = string.Empty;
        
        public string RtspUrl { get; set;} = string.Empty;

    }


}
