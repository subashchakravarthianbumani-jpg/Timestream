using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class DivisionListAPIResponceMode
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Division> data { get; set; }
    }

    public class Division
    {
        public string DivisionID { get; set; }
        public string Division_name { get; set; }
        public List<District> Districts { get; set; }
    }

    public class District
    {
        public string district_id { get; set; }
        public string district_name { get; set; }
        public string district_code { get; set; }
        public string division_id { get; set; }
    }

    public class WorkTypeConfigurationModel
    {
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string ConfigurationId { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string DeletedBy { get; set; }
        public string DeletedByUserName { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string DepartmentId { get; set; }
    }


}
