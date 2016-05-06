using JohnsonNet.log4net.Appender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Test.Console
{
    public class JohnsonSmtpAppenderTemplate : IJohnsonSmtpAppenderTemplate
    {
        public string GetBody()
        {
            var directory = System.IO.Directory.GetCurrentDirectory();
            var templatePath = Path.Combine(directory, "LogMailTemplate.html");
            var templateContent = File.ReadAllText(templatePath);

            return templateContent;
        }

        public string GetSubject()
        {
            return "Test A";
        }
    }
}
