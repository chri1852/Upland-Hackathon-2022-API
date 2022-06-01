using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes
{
    public class Property
    {
        public long id { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public Neighborhood neighborhood { get; set; }
    }
}
