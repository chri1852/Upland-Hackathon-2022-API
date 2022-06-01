using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes;
using UplandHackaton2022.Api.Abstractions;

namespace UplandHackathon2022.API.Infrastructure.Repositories
{
    public class UplandThirdPartyApiRepository : IUplandThirdPartyApiRepository
    {
        private HttpClient httpClient;
        private readonly IConfiguration _configuration;
        private const string baseUrl = @"https://api.sandbox.upland.me/developers-api/";
        private string basicAuthToken;

        public UplandThirdPartyApiRepository(IConfiguration configuration)
        {
            _configuration = configuration;

            this.basicAuthToken = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_configuration.GetSection("AppSettings")["Id"] + ":" + _configuration.GetSection("AppSettings")["SecretKey"]));
            
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            this.httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
        }

        public async Task<PostUserAppAuthResponse> PostOTP()
        {
            PostUserAppAuthResponse response;
            string requestUri = baseUrl + @"auth/otp/init";

            response = await PostApi<PostUserAppAuthResponse>(requestUri, null);

            return response;
        }

        public async Task LockContainer(int containerId)
        {
            string requestUri = baseUrl + "containers/" + containerId + "/lock";

            await PostApi<Task>(requestUri, null);
        }

        public async Task<EscrowContainer> PostNewEscrowContainer(int expirationTimeInHours)
        {
            EscrowContainer escrow;
            string requestUri = baseUrl + @"containers";

            PostCreateEscrowContainer request = new PostCreateEscrowContainer();
            request.expirationPeriodHours = expirationTimeInHours;

            escrow = await PostApi<EscrowContainer>(requestUri, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            return escrow;
        }

        public async Task<EscrowContainer> PostRefreshEscrowContainer(int containerId)
        {
            EscrowContainer escrow;
            string requestUri = baseUrl + @"containers/" + containerId + "/refresh-expiration-time";

            escrow = await PostApi<EscrowContainer>(requestUri, null);

            return escrow;
        }

        public async Task<EscrowContainer> GetEscrowContainerById(int containerId)
        {
            EscrowContainer escrow;
            string requestUri = baseUrl + @"containers/" + containerId;

            escrow = await GetApi<EscrowContainer>(requestUri);

            return escrow;
        }

        public async Task<PostEscrowResolveResponse> PostResolveEscrowContainer(int containerId, List<EscrowAction> escrowActions)
        {
            PostEscrowResolveRequest request = new PostEscrowResolveRequest();
            PostEscrowResolveResponse response = new PostEscrowResolveResponse();
            request.actions = escrowActions;

            string requestUri = baseUrl + @"containers/" + containerId + "/resolve";

            response = await PostApi<PostEscrowResolveResponse>(requestUri, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            return response;
        }

        public async Task<EscrowContainer> PostRefundEscrowContainer(int containerId)
        {
            EscrowContainer escrow;
            string requestUri = baseUrl + @"containers/" + containerId + "/refund";

            escrow = await PostApi<EscrowContainer>(requestUri, null);

            return escrow;
        }

        public async Task<GetUserProfileResponse> GetUserProfile(string authToken)
        {
            GetUserProfileResponse response = new GetUserProfileResponse();

            string requestUri = baseUrl + @"user/profile";

            response = await GetApi<GetUserProfileResponse>(requestUri, authToken);

            return response;
        }

        public async Task<GetUserNFTsResponse> GetUserNFTAssets(string authToken, int page, int pageSize, List<string> categories)
        {
            GetUserNFTsResponse response = new GetUserNFTsResponse();

            string requestUri = baseUrl + @"user/assets/nfts?currentPage=" + page + "&pageSize=" + pageSize;

            foreach(string entry in categories)
            {
                requestUri += "&categories[]=" + entry;
            }

            response = await GetApi<GetUserNFTsResponse>(requestUri, authToken);

            return response;
        }

        public async Task<GetUserBalancesResponse> GetUserBalances(string authToken)
        {
            GetUserBalancesResponse response = new GetUserBalancesResponse();

            string requestUri = baseUrl + @"user/balances";

            response = await GetApi<GetUserBalancesResponse>(requestUri, authToken);

            return response;
        }

        public async Task<GetUserPropertyResponse> GetUserProperties(string authToken, int page, int pageSize)
        {
            GetUserPropertyResponse response = new GetUserPropertyResponse();

            string requestUri = baseUrl + @"user/assets/properties?currentPage=" + page + "&pageSize=" + pageSize;

            response = await GetApi<GetUserPropertyResponse>(requestUri, authToken);

            return response;
        }

        public async Task<PostUserJoinEscrowResponse> PostJoinEscrow(string authToken, PostUserJoinEscrow request)
        {
            PostUserJoinEscrowResponse response = new PostUserJoinEscrowResponse();

            string requestUri = baseUrl + @"user/join";

            response = await PostApi<PostUserJoinEscrowResponse>(requestUri, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"), authToken);

            return response;
        }


        private async Task<T> GetApi<T>(string requestUri, string authToken = null)
        {
            HttpResponseMessage httpResponse;
            string responseJson;

            if (authToken == null)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", this.basicAuthToken);
            }
            else
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            httpResponse = await this.httpClient.GetAsync(requestUri);

            if (httpResponse.IsSuccessStatusCode)
            {
                responseJson = await httpResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            else
            {
                throw new Exception(httpResponse.StatusCode.ToString());
            }
        }

        private async Task<T> PostApi<T>(string requestUri, HttpContent content, string authToken = null)
        {
            HttpResponseMessage httpResponse;
            string responseJson;

            if (authToken == null)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", this.basicAuthToken);
            }
            else
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            httpResponse = await this.httpClient.PostAsync(requestUri, content);

            if (httpResponse.IsSuccessStatusCode)
            {
                responseJson = await httpResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
            else
            {
                throw new Exception(httpResponse.StatusCode.ToString());
            }
        }
    }
}
