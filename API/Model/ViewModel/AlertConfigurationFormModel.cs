using Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class AlertConfigurationFormModel
    {
        public List<SelectListItem> TypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ObjectList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SeverityList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FieldList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> BaseFieldList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CalculationTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> FrequencyTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UserGroupList { get; set; } = new List<SelectListItem>();
    }
}
