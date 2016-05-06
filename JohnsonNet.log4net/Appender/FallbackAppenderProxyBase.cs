#region licence
//  Copyright 2009 Michael Cromwell

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using JohnsonNet.log4net.Appender;
using log4net.Util;
using log4net.Core;

namespace JohnsonNet.log4net.Appender
{
    /// <summary>
    /// Provides the base class for the appender proxies use with <see cref="FallbackAppender"/>
    /// </summary>
    /// <author>Michael Cromwell</author>
    public abstract class FallbackAppenderProxyBase
    {
        protected AppenderSkeleton innerAppender;
        protected RecordingErrorHandler errorHandler;
        protected bool firstTimeThrough = true;

        public FallbackAppenderProxyBase(IAppender appenderToWrap)
        {
            var convertedAppender = appenderToWrap as AppenderSkeleton;
            if (convertedAppender == null)
                throw new InvalidOperationException("cannot use IndefiniteAppenderProxy with an appender that does not inherit from AppenderSkeleton as it needs to hook into the IErrorHandler, to gather errors.");

            innerAppender = convertedAppender;
            errorHandler = new RecordingErrorHandler(
                            new OnlyOnceErrorHandler());
            convertedAppender.ErrorHandler = errorHandler;
        }

        /// <summary>
        /// Appender being wrapped
        /// </summary>
        public AppenderSkeleton Appender
        {
            get
            {
                return innerAppender;
            }
        }

        /// <summary>
        /// Attempts to append to wrapped appender
        /// </summary>
        /// <returns>Whether the append was successful</returns>
        public bool TryAppend(LoggingEvent loggingEvent)
        {
            return DoAppend(() => innerAppender.DoAppend(loggingEvent));
        }

        /// <summary>
        /// Attempts to append to wrapped appender
        /// </summary>
        /// <returns>Whether the append was successful</returns>
        public bool TryAppend(LoggingEvent[] loggingEvents)
        {
            return DoAppend(() => innerAppender.DoAppend(loggingEvents));
        }

        /// <summary>
        /// Implemented by subclasses to provide their own behaviour
        /// </summary>
        /// <returns>Whether the append was successful</returns>
        protected abstract bool DoAppend(Action appendAction);
    }
}
