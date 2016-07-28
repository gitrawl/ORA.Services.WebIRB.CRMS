using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace ORA.Services.WebIRBCRMS.Shared
{
    public static class SharedFunctions
    {
        public static string GetValidUID(string UID)
        {
            if (string.IsNullOrEmpty(UID) || !Regex.IsMatch(UID, @"^[0-9]{9}$"))
                return null;
            else
                return UID;
        }

        public static string RemoveIrbNumberPrefix( string IrbNumber )
        {
            return IrbNumber.ToUpper().Replace("IRB", "").Replace("#", "");
        }
    }
}