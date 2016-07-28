using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ORA.Services.WebIRBCRMS.Security
{
    public class APIKey
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public String[] Roles { get; set; }
    }
}