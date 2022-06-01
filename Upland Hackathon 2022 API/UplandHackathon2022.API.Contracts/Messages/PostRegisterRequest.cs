using System.ComponentModel.DataAnnotations;

namespace UplandHackathon2022.API.Contracts.Messages
{
    public class PostRegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}