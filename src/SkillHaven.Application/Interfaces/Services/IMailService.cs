using SkillHaven.Shared.User.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task<(bool,string)> SendEmail(MailInfo mailData );
    }
}
