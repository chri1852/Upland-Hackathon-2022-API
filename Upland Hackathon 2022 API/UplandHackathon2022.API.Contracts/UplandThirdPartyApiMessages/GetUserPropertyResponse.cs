using System;
using System.Collections.Generic;
using System.Text;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class GetUserPropertyResponse
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalResults { get; set; }
        public List<Property> results { get; set; }
}
}
