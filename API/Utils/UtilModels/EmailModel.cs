using Utils.Interface;

namespace Utils.UtilModels
{
    public class EmailModel
    {
        public string Body { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public List<string> To { get; set; } = new List<string>();
        public List<string> CC { get; set; } = new List<string>();
        public List<string> BCC { get; set; } = new List<string>();
        public Dictionary<string, string> BodyPlaceHolders { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SubjectPlaceHolders { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, MemoryStream>? Attachemnts { get; set; }

        // Application
        public string Type { get;set; } = string.Empty;
        public string TypeId { get;set; } = string.Empty;
        public string ReceivedBy { get; set; } = string.Empty;
        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
