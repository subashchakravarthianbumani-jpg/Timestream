using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ResponseViewModel
    {
        public string? Status { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public int? TotalRecordCount { get; set; }
    }
}
