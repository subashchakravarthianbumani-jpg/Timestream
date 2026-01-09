using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class DepartmentAPIResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Department> data { get; set; }
    }
    public class Department
    {
        public string department_id { get; set; }
        public string department_name { get; set; }
        public string department_abv { get; set; }
        public string created_by { get; set; }
        public string created_at { get; set; }
        public string status { get; set; }
    }
}
