using JohnsonNet.Config;
using JohnsonNet.Serialization;
using RestSharp;
using System;
using System.Net.Mail;
using System.Text;

namespace JohnsonNet.Operation
{
    public class LogOperation
    {
        public Action<DateTime, string, string, string> SaveDatabaseAction { get; set; }
        private Func<string, string> p_GetProjectName = (s) => s;
        public Func<string, string> GetProjectName
        {
            get
            {
                return p_GetProjectName;
            }
            set
            {
                p_GetProjectName = value;
            }
        }

        public string Serialize(object input)
        {
            if (input == null) return null;

            try
            {
                var serializer = new JsonSerializer();
                return serializer.Serialize(input);
            }
            catch (Exception ex)
            {
                Log(ex, "Coundn't serialize {0}", input.GetType().FullName);
                return null;
            }
        }
        public bool Log(object input, string extra = null)
        {
            return Log(input, extra, null);
        }
        public bool Log(object input, string extra, params object[] args)
        {
            return Log(Serialize(input), extra);
        }
        public bool Log(string ex, string extra = null)
        {
            return Log(ex, extra, null);
        }
        public bool Log(Exception ex, string extra = null)
        {
            return Log(ex.ToString(), extra, null);
        }
        public bool Log(Exception ex, object extra)
        {
            return Log(ex.ToString(), Serialize(extra), null);
        }
        public bool Log(string ex, object extra)
        {
            return Log(ex, Serialize(extra), null);
        }
        public bool Log(Exception ex, string extra, params object[] args)
        {
            return Log(ex.ToString(), extra, args);
        }
        public bool Log(string exception, string extra, params object[] args)
        {
            DateTime date = DateTime.Now;
            try
            {
                var p = JohnsonManager.Config.Current;
                string logSmtpUser = p.GetSetting("LogSmtpUser", p.GetSetting<string>("SmtpUser"))
                     , logSmtpPass = p.GetSetting("LogSmtpPass", p.GetSetting<string>("SmtpPass"))
                     , logSmtpPort = p.GetSetting("LogSmtpPort", p.GetSetting<string>("SmtpPort"))
                     , logSmtpServer = p.GetSetting("LogSmtpServer", p.GetSetting<string>("SmtpServer"))
                     , logSmtpEnableSSL = p.GetSetting("LogEnableSSL", p.GetSetting<string>("SmtpEnableSSL"))
                     , logSmtpLogUsers = p.GetSetting<string>("LogEmailUsers")
                     , logProjectName = GetProjectName(p.GetSetting<string>("LogProjectName"))
                     , logType = p.GetSetting<string>("LogType", string.Empty);

                bool isMailLogType = logType.Contains("SendMail");
                bool isDatabaseLogType = logType.Contains("SaveDatabase") && SaveDatabaseAction != null;
                bool isApiLogType = logType.Contains("SaveAPI") ? true : !(isMailLogType || isDatabaseLogType);

                extra = (string.IsNullOrEmpty(extra) ? null : args != null ? string.Format(extra, args) : extra);

                if (isMailLogType)
                {
                    using (SmtpClient client = new SmtpClient
                    {
                        Credentials = new System.Net.NetworkCredential(logSmtpUser, logSmtpPass),
                        Port = int.Parse(logSmtpPort),
                        Host = logSmtpServer,
                        EnableSsl = bool.Parse(logSmtpEnableSSL)
                    })
                    {
                        using (MailMessage msg = new MailMessage()
                        {
                            Subject = logProjectName + " - System Failier",
                        })
                        {
                            StringBuilder exceptionContent = new StringBuilder();
                            exceptionContent.AppendLine(string.Format("ServerDate : {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now));
                            exceptionContent.AppendLine(string.Format("Exception: {0}", exception));
                            exceptionContent.AppendLine(string.Format("Extras: {0}", extra));

                            msg.Body = exceptionContent.ToString();
                            msg.From = new MailAddress(logSmtpUser, logProjectName + " - Logger");

                            foreach (var email in logSmtpLogUsers.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                msg.To.Add(email);
                            }
                            client.Send(msg);
                        }
                    }
                }
                if (isDatabaseLogType)
                {
                    SaveDatabaseAction(date, exception, extra, logProjectName);
                }
                if (isApiLogType)
                {
                    string LogAPIAddress = null;
                    var o = new
                    {
                        date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffff"),
                        exception = exception,
                        extra = extra,
                        projectName = logProjectName
                    };

                    var client = new RestClient(LogAPIAddress);
                    var request = new RestRequest("/logs/log/", Method.POST) { RequestFormat = DataFormat.Json }
                        .AddBody(o);

                    var response = client.Execute<ElasticSearchResult>(request);

                    if (response.Data == null ? true : !response.Data.Created) throw new Exception();
                }

                return true;
            }
            catch
            {
                string projectName = null;
                try { projectName = JohnsonManager.Config.Current.GetSetting<string>("LogProjectName"); }
                catch { projectName = "JohnsonNetLog"; }

                EventLog(date, exception, extra, projectName);
                return false;
            }
        }

        public bool EventLog(DateTime date, string exception, string extra, string projectName)
        {
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists(projectName))
                    System.Diagnostics.EventLog.CreateEventSource(projectName, "Application");
                string message = string.Format("Exception: {0}\r\nExtra:{1}", exception, extra);
                System.Diagnostics.EventLog.WriteEntry(projectName, message, System.Diagnostics.EventLogEntryType.Error);
                return true;
            }
            catch { return false; }
        }

        private class ElasticSearchResult
        {
            public bool Created { get; set; }
        }
    }
}
