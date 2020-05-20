

namespace buying_order_server.Data.Entity
{
    public class AppConfiguration : EntityBase
    {
        public long Id { get; set; }
        public string AppEmailName { get; set; }
        public string AppEmailUser { get; set; }
        public string AppEmailPassword { get; set; }
        public string AppBlacklist { get; set; }
        public string AppSMTPAddress { get; set; }
        public int AppSMTPPort { get; set; }
        public bool AppSMTPSecure { get; set; }
        public string AppEmailFrom { get; set; }
        public string AppEmailSubject { get; set; }
        public string AppEmailText { get; set; }
        public string AppEmailHtml { get; set; }
        public string AppCronPattern { get; set; }
        public string AppReplyLink { get; set; }
        public int AppNotificationTriggerDelta { get; set; }
    }
}
