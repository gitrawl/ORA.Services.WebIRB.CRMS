using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ORA.Services.WebIRBCRMS.Interfaces;
using Newtonsoft.Json;
using System.IO;

namespace ORA.Services.WebIRBCRMS.Security
{
    public class JSONAPIKeyVerifier : IApiKeyProvider
    {
        static List<APIKey> ValidAPIKeys = GetAPIKeys();

        private static List<APIKey> GetAPIKeys()
        {
            // Uses JSON.NET Serializer + StreamReader
            using (var s = new StreamReader(HttpContext.Current.Server.MapPath("~/App_Data/ApiKeys.json")))
            {
                var jtr = new JsonTextReader(s);
                var jsonSerializer = new JsonSerializer();
                return jsonSerializer.Deserialize<List<APIKey>>(jtr);
            }
        }

        public bool IsAPIKeyValid( string APIKey )
        {
            if (string.IsNullOrEmpty(APIKey))
            {
                return false;
            }
            if (ValidAPIKeys.Any(k => k.Key == APIKey) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] GetRoles( string APIKey)
        {
            return (ValidAPIKeys.Where(k => k.Key == APIKey).Select(k => k.Roles).FirstOrDefault());
        }
    }
}