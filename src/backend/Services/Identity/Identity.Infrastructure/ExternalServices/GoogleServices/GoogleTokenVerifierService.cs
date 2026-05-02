using Identity.Application.Common.Interfaces.IExternalServices.IGoogleServices;
using Identity.Common.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Infrastructure.ExternalServices.GoogleServices
{
    public class GoogleTokenVerifierService : IGoogleTokenVerifierService
    {
        private readonly GoogleAuthSettings _googleAuthSettings;

        public GoogleTokenVerifierService(IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _googleAuthSettings = googleAuthSettings.Value;
        }
    }
}
