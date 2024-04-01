using System.Net.Mail;

namespace WMBA_4.ViewModels
{
    
        public class EmailMessage
        {
            public List<EmailAddress> ToAddresses { get; set; } = new List<EmailAddress>();
            public string Subject { get; set; }
            public string Content { get; set; }
        }
    
}
