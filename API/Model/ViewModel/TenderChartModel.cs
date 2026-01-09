using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class TenderChartModel
    {
        public List<string> Labels {  get; set; } = new List<string>();
        public List<TenderChartDatasetModel> Datasets {  get; set; } = new List<TenderChartDatasetModel>();
    }
    public class TenderChartDatasetModel
    {
        public string Label { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public string BorderColor { get; set; } = string.Empty;
        public int BorderWidth { get; set; }
        public int BorderRadius { get; set; }
        public List<long> Data { get; set; } = new List<long>();
    }
}
