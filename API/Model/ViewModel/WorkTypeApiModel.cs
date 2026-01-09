using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class WorkTypeApiModel
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public WorkTypeData data { get; set; } // Not List<WorkTypeDetails>
    }

    public class WorkTypeData
    {
        public List<WorkTypeDetails> WorkTypeDetails { get; set; }
        public List<ServiceTypeDetails> ServiceType { get; set; }
        public List<CategoryTypeDetails> CategoryType { get; set; }
    }

    public class WorkTypeDetails
    {
        public string work_type_main_id { get; set; }
        public string work_type_main { get; set; }

        public string WorkTypeId { get; set; }
        public string work_type_sub { get; set; }
        public string work_type_sub_code { get; set; }
    }
    public class ServiceTypeDetails
    {
        public string service_type_main_id { get; set; }
        public string service_type_main { get; set; }
    }

    public class CategoryTypeDetails
    {
        public string category_type_main_id { get; set; }
        public string category_type_main { get; set; }
    }
}
