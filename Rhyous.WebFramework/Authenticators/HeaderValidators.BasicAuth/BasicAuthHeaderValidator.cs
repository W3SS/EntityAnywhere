﻿using Rhyous.WebFramework.Interfaces;
using Rhyous.WebFramework.Services;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Rhyous.WebFramework.HeaderValidators
{
    public class TokenValidator : IHeaderValidator
    {
        public long UserId { get; set; }
        
        public bool IsValid(NameValueCollection headers)
        {
            var basicAuthHeader = headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(basicAuthHeader))
            {
                var token = AuthService.Authenticate(new BasicAuth(basicAuthHeader).Creds);
                UserId = token.UserId;
            }
            return true;
        }

        #region Injectable
        public AuthenticationService AuthService
        {
            get { return _AuthService ?? (_AuthService = new AuthenticationService()); }
            set { _AuthService = value; }
        } private AuthenticationService _AuthService;
        #endregion
    }
}
