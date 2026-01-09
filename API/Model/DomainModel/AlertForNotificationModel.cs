using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    public class AlertForNotificationModel
    {
        public List<WorkMasterModel>? workMasters { get; set; }
        public List<WorkTemplateMilestoneMasterModel>? workTemplateMilestones { get; set; }
        public List<MBookMasterModel>? mBookMasters { get; set; }
        public List<AlertConfigurationPrimaryModel>? primaryModels { get; set; }
        public List<AlertConfigurationSecondaryModel>? secondaryModels { get; set; }
        public List<StatusMaster>? statuses { get; set; }
    }
}
