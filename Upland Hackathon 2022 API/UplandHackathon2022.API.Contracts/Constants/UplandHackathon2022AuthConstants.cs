using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Constants
{
    public class UplandHackathon2022AuthConstants
    {
        public const string UplandHackathon2022AuthScheme = "UplandHackathon2022";
        public const string NToken = "UplandHackathon2022 (?<token>.*)";
    }

    // TODO: CHANGE THESE
    public class UplandHackathon2022AuthClaimTypes
    {
        public const string UplandUsername = "UplandUsername";
        public const string RegisteredUserId = "RegisteredUserId";
        public const string PasswordHash = "PasswordHash";
        public const string WebVerified = "WebVerified";
    }
}

