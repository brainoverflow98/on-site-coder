using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Common.Environment
{
    public struct Claims
    {
        public const string Id = ClaimTypes.NameIdentifier;
        public const string Email = ClaimTypes.Email;
        public const string DisplayName = ClaimTypes.GivenName;
        public const string Role = ClaimTypes.Role;
    }    
}
