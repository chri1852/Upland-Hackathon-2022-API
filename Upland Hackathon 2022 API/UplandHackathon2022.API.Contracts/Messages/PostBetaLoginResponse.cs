using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Messages
{
    public class PostBetaLoginResponse
    {
        public string Message { get; set; }
        public string AuthToken { get; set; }
        public bool MustEnterCode { get; set; }
        public string OTPCode { get; set; }
    }
}