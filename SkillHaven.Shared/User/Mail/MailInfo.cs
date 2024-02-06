using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.User.Mail
{
    public class MailInfo
    {
        public string EmailToId { get; set; }
        public string EmailToName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

        public MailType MailType { get; set; }
    }

    public enum MailType
    {
        PlainText=1,
        Html=2
    }
}
