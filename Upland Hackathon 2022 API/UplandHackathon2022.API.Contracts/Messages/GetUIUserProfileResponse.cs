using System;
using System.Collections.Generic;
using System.Text;
using UplandHackathon2022.API.Contracts.Types;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages;

namespace UplandHackathon2022.API.Contracts.Messages
{
    public class GetUIUserProfileResponse
    {
        public string Message { get; set; }
        public GetUserProfileResponse userProfile { get; set; }
    }
}
