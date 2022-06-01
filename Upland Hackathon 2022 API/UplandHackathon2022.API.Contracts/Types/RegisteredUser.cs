using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Types
{
    public class RegisteredUser
    {
        public int Id { get; set; }
        public string UplandUsername { get; set; }
        public Guid? UplandId { get; set; }
        public string? PasswordSalt { get; set; }
        public string? PasswordHash { get; set; }
        public string? UplandAccessToken { get; set; }
        public string AccessCode { get; set; }
    }
}
