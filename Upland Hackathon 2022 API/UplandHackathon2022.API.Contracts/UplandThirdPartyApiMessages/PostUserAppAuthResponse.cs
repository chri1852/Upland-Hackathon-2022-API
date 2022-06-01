using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class PostUserAppAuthResponse
    {
        public string code { get; set; }
        public DateTime expireAt { get; set; }
    }
}
