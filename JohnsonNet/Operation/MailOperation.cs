using JohnsonNet.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace JohnsonNet.Operation
{
    public class MailOperation
    {
        public string SmtpServer { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSSL { get; set; }

        public Exception LastException { get; set; }

        public bool Send(string to
            , string body
            , string subject
            , bool isBodyHtml = true
            , string cc = null
            , string bcc = null
            , string fromMail = null
            , string fromDisplayName = null
            , string replyToMail = null
            , string replyToDisplayName = null
            , string smtpServer = null
            , string smtpUser = null
            , string smtpPass = null
            , int smtpPort = 0
            , bool enableSSL = false
            , List<Attachment> attachments = null)
        {
            var f = Provider.Config;
            smtpServer = Core.Default(smtpServer, Core.Default(SmtpServer, f.GetSetting("SmtpServer")));
            smtpUser = Core.Default(smtpUser, Core.Default(SmtpUser, f.GetSetting("SmtpUser")));
            smtpPass = Core.Default(smtpPass, Core.Default(SmtpPass, f.GetSetting("SmtpPass")));
            smtpPort = Core.Default(smtpPort, Core.Default(SmtpPort, f.GetSetting<int>("SmtpPort")));
            enableSSL = Core.Default(enableSSL, Core.Default(EnableSSL, f.GetSetting<bool>("SmtpEnableSSL")));

            fromMail = Core.Default(Core.Default(fromMail, f.GetSetting("SmtpFromMail")), smtpUser);
            fromDisplayName = Core.Default(fromDisplayName, f.GetSetting("SmtpUserDisplayName"));
            replyToMail = Core.Default(replyToMail, f.GetSetting("SmtpReplyToMail"));
            replyToDisplayName = Core.Default(replyToDisplayName, f.GetSetting("SmtpReplyToDisplayName"));

            using (SmtpClient client = new SmtpClient
            {
                Port = smtpPort,
                Host = smtpServer,
                EnableSsl = enableSSL,
            })
            {
                if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPass))
                {
                    client.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass);
                }

                using (MailMessage msg = new MailMessage()
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                })
                {
                    if (!string.IsNullOrEmpty(fromMail))
                    {
                        msg.From = new MailAddress(fromMail, fromDisplayName);
                    }
                    if (!string.IsNullOrEmpty(replyToMail))
                    {
                        msg.ReplyToList.Add(new MailAddress(replyToMail, replyToDisplayName));
                    }

                    if (!string.IsNullOrEmpty(to))
                    {
                        foreach (var item in to.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            msg.To.Add(item);
                        }
                    }
                    if (!string.IsNullOrEmpty(cc))
                    {
                        foreach (var item in cc.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            msg.CC.Add(item);
                        }
                    }

                    if (!string.IsNullOrEmpty(bcc))
                    {
                        foreach (var item in bcc.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            msg.Bcc.Add(item);
                        }
                    }

                    if (attachments != null && attachments.Count > 0)
                        foreach (var item in attachments.ToList())
                            msg.Attachments.Add(item);
                    try
                    {
                        client.Send(msg);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LastException = ex;
                        return false;
                    }
                }
            }
        }

        public static bool IsEmail(string email)
        {
            try
            {
                MailAddress a = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
