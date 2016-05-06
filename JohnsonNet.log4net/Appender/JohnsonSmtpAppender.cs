using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;
using System.IO;

namespace JohnsonNet.log4net.Appender
{
    public class JohnsonSmtpAppender : global::log4net.Appender.AppenderSkeleton
    {
        protected override bool RequiresLayout
        {
            get
            {
                return false;
            }
        }
        public string TemplateTypeName { get; set; }

        public string LogMailerPrefix { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            var templateTypeName = TemplateTypeName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var templateAssemblyName = TemplateTypeName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1];

            if (string.IsNullOrEmpty(LogMailerPrefix)) LogMailerPrefix = string.Empty;

            var operation = new Operation.MailOperation();
            var template = Activator.CreateInstance(templateAssemblyName, templateTypeName).Unwrap() as IJohnsonSmtpAppenderTemplate;
            var templateSubject = template.GetSubject();
            var templateBody = template.GetBody();
            var patternLayout = new global::log4net.Layout.PatternLayout(templateBody);

            var to = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "MailTo");
            var cc = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "MailCC");
            var bcc = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "MailBCC");

            operation.SmtpServer = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "SmtpServer");
            operation.SmtpUser = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "SmtpUser");
            operation.SmtpPass = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "SmtpPass");
            operation.SmtpPort = JohnsonManager.Config.Current.GetSetting<int>(LogMailerPrefix + "SmtpPort");
            operation.SmtpEnableSSL = JohnsonManager.Config.Current.GetSetting<bool>(LogMailerPrefix + "SmtpEnableSSL");
            operation.FromMail = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "FromMail");
            operation.FromDisplayName = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "FromDisplayName");
            operation.ReplyToMail = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "ReplyToMail");
            operation.ReplyToDisplayName = JohnsonManager.Config.Current.GetSetting(LogMailerPrefix + "ReplyToDisplayName");

            var mailBody = patternLayout.Format(loggingEvent);
            var ex = operation.Send(to, templateSubject, mailBody, cc: cc, bcc: bcc);

            if (ex != null)
            {
                throw new LogException("Failed to send mail", ex);
            }
        }
    }
}
