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

namespace JohnsonNet.log4net
{
    /// <summary>
    /// Provides an abstraction for the system clock
    /// </summary>
    public static class SystemDateTime
    {
        /// <summary>
        /// By default returns the current date and time but can be set for
        /// unit testing purposes
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.Now;
    }
}
