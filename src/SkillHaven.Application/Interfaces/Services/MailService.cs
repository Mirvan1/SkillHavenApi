using MimeKit;
using SkillHaven.Shared.User.Mail;
using System;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;

namespace SkillHaven.Application.Interfaces.Services
{
    public class MailService:IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettingsOptions)
        {
            _mailSettings = mailSettingsOptions.Value;
        }


        public async Task<bool> SendEmail(MailInfo mailData)
        {//MirvanSdql@2023!.
            using (MimeMessage emailMessage = new MimeMessage())
            {
                MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress(mailData.EmailToName, mailData.EmailToId);
                emailMessage.To.Add(emailTo);

                emailMessage.Cc.Add(new MailboxAddress("Cc Receiver", "cc@example.com"));
                emailMessage.Bcc.Add(new MailboxAddress("Bcc Receiver", "bcc@example.com"));

                emailMessage.Subject = mailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new BodyBuilder();

                if(mailData.MailType==MailType.PlainText)
                    emailBodyBuilder.TextBody = mailData.EmailBody;
                else if(mailData.MailType==MailType.Html)
                    emailBodyBuilder.HtmlBody = mailData.EmailBody;

                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                using (SmtpClient mailClient = new SmtpClient())
                {
                    await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    await mailClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                    await mailClient.SendAsync(emailMessage);
                    await mailClient.DisconnectAsync(true);
                }
            }

            return true;
        }
    }
}
