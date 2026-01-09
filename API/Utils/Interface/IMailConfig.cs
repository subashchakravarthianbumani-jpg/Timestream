namespace Utils.Interface
{
    public interface IMailConfig
    {
        string Body { get; set; }
        string Subject { get; set; }
        IEnumerable<string> To { get; set; }
        IEnumerable<string> CC { get; set; }
        IEnumerable<string> BCC { get; set; }
        IDictionary<string, string> BodyPlaceHolders { get; set; }
        IDictionary<string, string> SubjectPlaceHolders { get; set; }

    }
}
