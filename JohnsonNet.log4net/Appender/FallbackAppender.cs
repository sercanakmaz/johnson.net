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
using log4net.Util;

namespace JohnsonNet.log4net.Appender
{
    #region enums

    /// <summary>
    /// Used to determine how the <see cref="FallbackAppender"/> deals with appenders
    /// that have caused an error
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term><c>Indefinite</c></term>
    /// <term><c>Time</c></term>
    /// <term><c>Count</c></term>
    /// </listheader>
    /// <item>
    /// <description>
    /// Indefinite will mean that once an appender has had an error that appender
    /// will not be appended to indefinitely.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Time will mean that once an appender has had an error the time will be recorded
    /// and each time subsequent appends are attempted a check will be made to see if the
    /// time the error was recorded is after the specified <see cref="FallbackAppender.MinutesTimeout"/> if
    /// so the appender will be reset as having an error and will be used once again.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Count will mean that once an appender has had an error subsequent appends will be
    /// counted and if the number of counts reaches the <see cref="FallbackAppender.AppendCount"/> the
    /// appender will be reset as having an error and will be used once again.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public enum FallbackAppenderMode
    {
        Indefinite,
        Time,
        Count
    }

    #endregion

    /// <summary>
    /// This appender takes care of falling back to another appender if appending causes
    /// an error
    /// </summary>
    /// <remarks>
    /// <para>
    /// The appenders are checked in the order they are referenced  in the XML
    /// </para>
    /// <example>
    /// example of XML declaration
    /// <code lang="XML" escaped="true">
    /// <appender name="FallbackAppender" type="JohnsonNet.log4net.Appender.FallbackAppender, log4net.FallbackAppender" >
    ///     <appender-ref ref="FileAppender" />
    ///     <appender-ref ref="ConsoleAppender" />
    ///     <mode value="time"/>
    ///     <minutesTimeout value="10" />
    /// </appender>
    /// </code>
    /// In this example if FileAppender caused an error the append will fallback to ConsoleAppender
    /// </example>
    /// <para>
    /// <seealso cref="FallbackAppenderMode"/>
    /// </para>
    /// </remarks>
    /// <author>Michael Cromwell</author>
    public class FallbackAppender : ForwardingAppender
    {
        protected IList<FallbackAppenderProxyBase> safeAppenderList;
        protected IDictionary<FallbackAppenderMode, Func<IAppender, FallbackAppenderProxyBase>> appenderModeMap;
        protected FallbackAppenderMode mode = FallbackAppenderMode.Indefinite;
        protected int minutesTimeout = 5;
        protected int appendCount = 20;

        public FallbackAppender()
        {
            appenderModeMap = new Dictionary<FallbackAppenderMode, Func<IAppender, FallbackAppenderProxyBase>>(
                                Enum.GetNames(typeof(FallbackAppenderMode)).Length);

            appenderModeMap.Add(FallbackAppenderMode.Indefinite, x => new IndefiniteAppenderProxy(x));
            appenderModeMap.Add(FallbackAppenderMode.Time, x => new TimeAppenderProxy(x, minutesTimeout));
            appenderModeMap.Add(FallbackAppenderMode.Count, x => new CountAppenderProxy(x, appendCount));
        }

        /// <summary>
        /// Wraps the appenders in the corresponding <see cref="FallbackAppenderProxyBase"/>
        /// implementation using the <see cref="FallbackAppenderMode"/> selected
        /// </summary>
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            safeAppenderList = new List<FallbackAppenderProxyBase>(Appenders.Count);
            foreach (var appender in Appenders)
                safeAppenderList.Add(appenderModeMap[mode](appender));
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var appenderQueue = new Queue<FallbackAppenderProxyBase>(safeAppenderList);
            while (appenderQueue.Count > 0)
            {
                var appender = appenderQueue.Dequeue();

                if (appender.TryAppend(loggingEvent))
                    break;

                RecordAppenderError(appenderQueue, appender);
            }
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (loggingEvents == null)
                throw new ArgumentNullException("loggingEvents");

            if (loggingEvents.Length == 0)
                throw new ArgumentException("loggingEvents array must not be empty", "loggingEvents");

            if (loggingEvents.Length == 1)
            {
                Append(loggingEvents[0]);
                return;
            }

            var appenderQueue = new Queue<FallbackAppenderProxyBase>(safeAppenderList);
            while (appenderQueue.Count > 0)
            {
                var appender = appenderQueue.Dequeue();

                var bulkAppender = appender.Appender as IBulkAppender;
                if (bulkAppender != null)
                {
                    if (appender.TryAppend(loggingEvents))
                        break;

                    RecordAppenderError(appenderQueue, appender);
                }
                else
                {
                    foreach (var logEvent in loggingEvents)
                        if (appender.TryAppend(logEvent))
                            break;

                    RecordAppenderError(appenderQueue, appender);
                }
            }
        }

        private static void RecordAppenderError(Queue<FallbackAppenderProxyBase> appenderQueue, FallbackAppenderProxyBase appender)
        {
            LogLog.Error(appender.Appender.GetType(), "appender [" + appender.Appender.Name + "] has an error so is not being appended to.");
            if (appenderQueue.Count > 0)
            {
                var nextAppender = appenderQueue.Peek();
                LogLog.Debug(appender.Appender.GetType(), "Chaining through to appender [" + nextAppender.Appender.Name + "]");
            }
            else
                LogLog.Error(appender.Appender.GetType(), "No more appenders exist to chain through to");
        }

        /// <summary>
        /// Sets the <see cref="FallbackAppender"/> into a specific <see cref="FallbackAppenderMode"/>
        /// </summary>
        public FallbackAppenderMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Used in conjuction with <see cref="FallbackAppenderMode.Time"/> to specify
        /// the amount of minutes timeout to wait for before resetting that an error occurred
        /// on an appender.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Must be greater than 0
        /// </para>
        /// <para>
        /// see <seealso cref="FallbackAppenderMode"/>
        /// </para>
        /// </remarks>
        public int MinutesTimeout
        {
            get { return minutesTimeout; }
            set
            {
                if (value < 1)
                {
                    ErrorHandler.Error(string.Format("MinutesTimeout was set to {0} which is below zero, so will be ignored.", value));
                    return;
                }

                minutesTimeout = value;
            }
        }

        /// <summary>
        /// Used in conjunction with <see cref="FallbackAppenderMode.Count"/> to specify
        /// the amount of counts to use before resetting that and error occurred on an appender.
        /// </summary>
        /// <remarks>
        /// <para>Must be greater than 0</para>
        /// <para>
        /// see <seealso cref="FallbackAppenderMode"/>
        /// </para>
        /// </remarks>
        public int AppendCount
        {
            get { return appendCount; }
            set
            {
                if (value < 1)
                {
                    ErrorHandler.Error(string.Format("AppendCount was set to {0} which is below zero, so will be ignored.", value));
                    return;
                }

                appendCount = value;
            }
        }
    }
}
