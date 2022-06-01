using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class UserConnectionRequest
    {
        public string type { get; set; }
        public UserConnectionRequestData data { get; set; }
    }

    public class UserConnectionRequestData
    {
        public string code { get; set; }
        public Guid userId { get; set; }
        public string accessToken { get; set; }
    }
}
