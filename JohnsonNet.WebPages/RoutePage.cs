using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web.WebPages
{
    public class RoutePage
    {
        public string PagePath { get; set; }
        public string Permalink { get; set; }
        public string Module { get; set; }
        public bool Authenticate { get; set; }
    }
}
