using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;
using System.IO;
using System.Web.Compilation;
using System.Reflection;
using System.Web;

namespace JohnsonNet.log4net
{
    public class ApplicationPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            Assembly assembly = null;
            if (HttpContext.Current != null)
            {
                assembly = BuildManager.GetGlobalAsaxType().BaseType.Assembly;
            }
            else
            {
                assembly = Assembly.GetEntryAssembly();
            }

            writer.Write(assembly.ManifestModule.Name
                .Replace(".dll", string.Empty)
                .Replace(".exe", string.Empty));
        }
    }
}
