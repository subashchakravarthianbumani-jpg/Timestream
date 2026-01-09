using Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ReportFilterForm
    {
        public List<SelectListItem>? types { get; set; }
        public List<SelectListItem>? divisions { get; set; }
        public List<SelectListItem>? districts { get; set; }
        public List<SelectListItem>? statusList { get; set; }
        public List<SelectListItem>? departments { get; set; }
        public List<SelectListItem>? schemes { get; set; }
        public List<SelectListItem>? GoPackageNo { get; set; }
    }
}
