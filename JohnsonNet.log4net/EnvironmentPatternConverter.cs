using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;
using System.IO;

namespace JohnsonNet.log4net
{
    public class EnvironmentPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(JohnsonManager.Config.CurrentEnvironment.ToString());
        }
    }
}
