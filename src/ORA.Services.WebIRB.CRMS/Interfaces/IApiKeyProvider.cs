using System;

namespace ORA.Services.WebIRBCRMS.Interfaces
{
    public interface IApiKeyProvider
    {
        bool IsAPIKeyValid( string APIKey );
        string[] GetRoles( string APIKey );
    }
}
