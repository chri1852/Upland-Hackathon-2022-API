using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.Constants;
using UplandHackathon2022.API.Contracts.Types;

namespace UplandHackathon2022.API.Infrastructure.Authentication
{
    public class UplandHackathon2022AuthHandler : AuthenticationHandler<UplandHackathon2022AuthSchemeOptions>
    {
        public UplandHackathon2022AuthHandler(
            IOptionsMonitor<UplandHackathon2022AuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            UplandHackathon2022AuthToken token;

            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Header Not Found."));
            }

            string header = Request.Headers[HeaderNames.Authorization].ToString();
            var tokenMatch = Regex.Match(header, UplandHackathon2022AuthConstants.NToken);

            if (tokenMatch.Success)
            {
                string tokenString = tokenMatch.Groups["token"].Value;

                try
                {
                    byte[] fromBase64String = Convert.FromBase64String(tokenString);
                    string parsedToken = Encoding.UTF8.GetString(fromBase64String);

                    token = JsonConvert.DeserializeObject<UplandHackathon2022AuthToken>(parsedToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception Occured while Deserializing: " + ex);
                    return Task.FromResult(AuthenticateResult.Fail("TokenParseException"));
                }

                if (token != null)
                {
                    List<Claim> claims = new List<Claim> {
                        new Claim("RegisteredUserId", token.RegisteredUserId.ToString()),
                        new Claim("UplandUsername", token.UplandUsername),
                        new Claim("PasswordHash", token.PasswordHash),
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, nameof(UplandHackathon2022AuthHandler));

                    AuthenticationTicket ticket = new AuthenticationTicket(
                        new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

            return Task.FromResult(AuthenticateResult.Fail("Model is Empty"));
        }
    }
}