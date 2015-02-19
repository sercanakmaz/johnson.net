using JohnsonNet.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace JohnsonNet.Operation
{
    public class MailOperation
    {
        private string p_SmtpServer = null;
        public string SmtpServer
        {
            get
            {
                if (string.IsNullOrEmpty(p_SmtpServer))
                {
                    p_SmtpServer = JohnsonManager.Config.Current.GetSetting("SmtpServer");
                }
                return p_SmtpServer;
            }
            set
            {
                p_SmtpServer = value;
            }
        }
        private string p_SmtpUser = null;
        public string SmtpUser
        {
            get
            {
                if (string.IsNullOrEmpty(p_SmtpUser))
                {
                    p_SmtpUser = JohnsonManager.Config.Current.GetSetting("SmtpUser");
                }
                return p_SmtpUser;
            }
            set
            {
                p_SmtpUser = value;
            }
        }
        private string p_SmtpPass = null;
        public string SmtpPass
        {
            get
            {
                if (string.IsNullOrEmpty(p_SmtpPass))
                {
                    p_SmtpPass = JohnsonManager.Config.Current.GetSetting("SmtpPass");
                }
                return p_SmtpPass;
            }
            set
            {
                p_SmtpPass = value;
            }
        }
        private int p_SmtpPort;
        public int SmtpPort
        {
            get
            {
                if (p_SmtpPort == default(int))
                {
                    p_SmtpPort = JohnsonManager.Config.Current.GetSetting<int>("SmtpPort");
                }
                return p_SmtpPort;
            }
            set
            {
                p_SmtpPort = value;
            }
        }
        private bool p_SmtpEnableSSL;
        public bool SmtpEnableSSL
        {
            get
            {
                if (p_SmtpEnableSSL == default(bool))
                {
                    p_SmtpEnableSSL = JohnsonManager.Config.Current.GetSetting<bool>("SmtpEnableSSL");
                }
                return p_SmtpEnableSSL;
            }
            set
            {
                p_SmtpEnableSSL = value;
            }
        }
        private string p_FromMail = null;
        public string FromMail
        {
            get
            {
                if (string.IsNullOrEmpty(p_FromMail))
                {
                    p_FromMail = JohnsonManager.Config.Current.GetSetting("FromMail");
                }
                return p_FromMail;
            }
            set
            {
                p_FromMail = value;
            }
        }
        private string p_FromDisplayName = null;
        public string FromDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(p_FromDisplayName))
                {
                    p_FromDisplayName = JohnsonManager.Config.Current.GetSetting("FromDisplayName");
                }
                return p_FromDisplayName;
            }
            set
            {
                p_FromDisplayName = value;
            }
        }
        private string p_ReplyToMail = null;
        public string ReplyToMail
        {
            get
            {
                if (string.IsNullOrEmpty(p_ReplyToMail))
                {
                    p_ReplyToMail = JohnsonManager.Config.Current.GetSetting("ReplyToMail");
                }
                return p_ReplyToMail;
            }
            set
            {
                p_ReplyToMail = value;
            }
        }
        private string p_ReplyToDisplayName = null;
        public string ReplyToDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(p_ReplyToDisplayName))
                {
                    p_ReplyToDisplayName = JohnsonManager.Config.Current.GetSetting("ReplyToDisplayName");
                }
                return p_ReplyToDisplayName;
            }
            set
            {
                p_ReplyToDisplayName = value;
            }
        }

        public Exception Send(string to
            , string subject
            , string body
            , bool isBodyHtml = true
            , string cc = null
            , string bcc = null
            , List<Attachment> attachments = null)
        {
            using (SmtpClient client = new SmtpClient
            {
                Port = SmtpPort,
                Host = SmtpServer,
                EnableSsl = SmtpEnableSSL,
            })
            {
                if (!string.IsNullOrEmpty(SmtpUser) && !string.IsNullOrEmpty(SmtpPass))
                {
                    client.Credentials = new System.Net.NetworkCredential(SmtpUser, SmtpPass);
                }

                using (MailMessage msg = new MailMessage()
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                })
                {
                    if (!string.IsNullOrEmpty(FromMail))
                    {
                        msg.From = new MailAddress(FromMail, FromDisplayName);
                    }
                    else
                    {
                        msg.From = new MailAddress(SmtpUser);
                    }

                    if (!string.IsNullOrEmpty(ReplyToMail))
                    {
                        msg.ReplyToList.Add(new MailAddress(ReplyToMail, ReplyToDisplayName));
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
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex;
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
