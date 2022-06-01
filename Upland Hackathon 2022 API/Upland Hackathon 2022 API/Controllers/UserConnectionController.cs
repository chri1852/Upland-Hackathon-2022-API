using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.Constants;
using UplandHackathon2022.API.Contracts.Messages;
using UplandHackathon2022.API.Contracts.Types;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages;
using UplandHackaton2022.Api.Abstractions;

namespace Upland_Hackathon_2022_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserConnectionController : ControllerBase
    {
        private readonly IUplandThirdPartyApiRepository _uplandThirdPartyApiRepository;
        private readonly ILocalRepository _localRepository;
        private readonly Random _random;

        public UserConnectionController(IUplandThirdPartyApiRepository thirdPartyRepo, ILocalRepository localRepo)
        {
            _uplandThirdPartyApiRepository = thirdPartyRepo;
            _localRepository = localRepo;
            _random = new Random();
        }

        [HttpPost]
        [AllowAnonymous]
        public void Post(UserConnectionRequest request)
        {
            RegisteredUser registeredUser = _localRepository.GetUserByAccessCode(request.data.code);

            if (registeredUser != null)
            {
                registeredUser.UplandId = request.data.userId;
                registeredUser.UplandAccessToken = request.data.accessToken;

                _localRepository.UpsertUser(registeredUser);
            }
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<PostUserAppAuthResponse>> Register(PostRegisterRequest request)
        {
            PostUserAppAuthResponse response = new PostUserAppAuthResponse();
            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(request.Username);

            if (registeredUser == null)
            {
                try
                {
                    PostUserAppAuthResponse authCode = await _uplandThirdPartyApiRepository.PostOTP();

                    registeredUser = new RegisteredUser();

                    registeredUser.UplandUsername = request.Username;
                    registeredUser.PasswordSalt = BuildSaltForUser(registeredUser);
                    registeredUser.PasswordHash = GetPasswordHash(registeredUser, request.Password);
                    registeredUser.AccessCode = authCode.code;

                    _localRepository.UpsertUser(registeredUser);

                    return Ok(authCode);
                }
                catch (Exception ex)
                {
                    return BadRequest("Failed Getting Auth Code");
                }
            }
            else
            {
                return BadRequest("User Exists");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public ActionResult<PostLoginResponse> Login(PostLoginRequest request)
        {
            PostLoginResponse response = new PostLoginResponse();
            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(request.Username);

            if (registeredUser == null || registeredUser.PasswordHash == null || registeredUser.PasswordSalt == null)
            {
                response.Message = "User Does Not Exist";
                return BadRequest(response);
            }

            if (!ValidatePassword(registeredUser, request.Password))
            {
                response.Message = "Username or Password is Incorrect";
                return BadRequest(response);
            }

            response.Message = "Success";
            response.AuthToken = GetEncodedToken(registeredUser);
            return Ok(response);
        }

        [HttpPost("BetaLogin")]
        [AllowAnonymous]
        public async Task<ActionResult<PostBetaLoginResponse>> BetaLogin(PostLoginRequest request)
        {
            PostBetaLoginResponse response = new PostBetaLoginResponse();
            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(request.Username);

            if (registeredUser == null || registeredUser.UplandAccessToken == null)
            {
                if (registeredUser == null)
                {
                    registeredUser = new RegisteredUser();

                    registeredUser.UplandUsername = request.Username;
                    registeredUser.PasswordSalt = BuildSaltForUser(registeredUser);
                    registeredUser.PasswordHash = GetPasswordHash(registeredUser, request.Password);
                }

                try
                {
                    PostUserAppAuthResponse authCode = await _uplandThirdPartyApiRepository.PostOTP();

                    registeredUser.AccessCode = authCode.code;
                    _localRepository.UpsertUser(registeredUser);
                    registeredUser = _localRepository.GetUserByUplandUsername(request.Username);
                    response.MustEnterCode = true;
                    response.OTPCode = registeredUser.AccessCode;
                }
                catch (Exception ex)
                {
                    return BadRequest("Failed Getting Auth Code");
                }
            }

            if (!ValidatePassword(registeredUser, request.Password))
            {
                response.Message = "Username or Password is Incorrect";
                return BadRequest(response);
            }

            response.Message = "Success";
            response.AuthToken = GetEncodedToken(registeredUser);
            return Ok(response);
        }

        #region Password Validation Functions

        private string GetEncodedToken(RegisteredUser user)
        {
            UplandHackathon2022AuthToken token = new UplandHackathon2022AuthToken
            {
                UplandUsername = user.UplandUsername,
                RegisteredUserId = user.Id,
                PasswordHash = user.PasswordHash
            };

            string encodedJsonToken = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(token)));

            return string.Format("{0} {1}", UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme, encodedJsonToken);
        }

        private string GetPasswordHash(RegisteredUser user, string password)
        {
            return PerformHashOnString(password, user.PasswordSalt);
        }

        private bool ValidatePassword(RegisteredUser user, string password)
        {
            return PerformHashOnString(password, user.PasswordSalt) == user.PasswordHash ? true : false;
        }

        private string BuildSaltForUser(RegisteredUser user)
        {
            return PerformHashOnString(CreateRandomString(), user.UplandUsername + CreateRandomString());
        }

        private string CreateRandomString()
        {
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randString = new char[8];

            for (int i = 0; i < randString.Length; i++)
            {
                randString[i] = characters[_random.Next(characters.Length)];
            }

            return randString.ToString();
        }

        private string PerformHashOnString(string stringToHash, string salt)
        {
            SHA256 shaHasher = SHA256.Create();

            byte[] tempHash = shaHasher.ComputeHash(Encoding.UTF32.GetBytes(stringToHash + salt));

            int i;
            StringBuilder sOutput = new StringBuilder(tempHash.Length);
            for (i = 0; i < tempHash.Length; i++)
            {
                sOutput.Append(tempHash[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
        #endregion Password Validation Functions


    }
}