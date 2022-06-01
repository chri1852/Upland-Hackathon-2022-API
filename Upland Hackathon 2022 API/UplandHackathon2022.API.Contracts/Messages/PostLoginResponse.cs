using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Messages
{
    public class PostLoginResponse
    {
        public string Message { get; set; }
        public string AuthToken { get; set; }
    }
}
