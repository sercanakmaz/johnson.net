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
using log4net.Core;

namespace JohnsonNet.log4net.Appender
{
    /// <summary>
    /// This appender proxy is chosen via the <see cref="FallbackAppenderMode.Time"/> mode
    /// from the <see cref="FallbackAppender.Mode"/> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// see <see cref="FallbackAppenderMode"/> for strategy details
    /// </para>
    /// </remarks>
    /// <author>Michael Cromwell</author>
    public class TimeAppenderProxy : FallbackAppenderProxyBase
    {
        protected int minutesTimeout;
        protected DateTime? errorOccurredTimestamp;

        /// <summary>
        /// Wraps up an <see cref="IAppender"/> adding extra behaviour to how to handle
        /// an error while appending
        /// </summary>
        /// <param name="minutesTimeout">Amount of minutes to wait before attempting to append again while has error</param>
        public TimeAppenderProxy(IAppender appenderToWrap, int minutesTimeout)
            : base(appenderToWrap)
        {
            this.minutesTimeout = minutesTimeout;
        }

        protected override bool DoAppend(Action appendAction)
        {
            if (firstTimeThrough)
            {
                appendAction();
                firstTimeThrough = false;
            }
            else
            {
                if ((errorOccurredTimestamp.HasValue) 
                    && (SystemDateTime.Now() >= errorOccurredTimestamp.Value.AddMinutes(minutesTimeout)))
                {
                    errorOccurredTimestamp = null;
                    errorHandler.ResetError();
                }

                if (!errorHandler.HasError)
                    appendAction();
            }

            if (errorHandler.HasError)
            {
                errorOccurredTimestamp = SystemDateTime.Now();
                return false;
            }
            else
                return true;
        }
    }
}
