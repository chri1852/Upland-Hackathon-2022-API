using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes;

namespace UplandHackaton2022.Api.Abstractions
{
    public interface IUplandThirdPartyApiRepository
    {
        Task<PostUserAppAuthResponse> PostOTP();

        Task LockContainer(int containerId);

        Task<EscrowContainer> PostNewEscrowContainer(int expirationTimeInHours);

        Task<EscrowContainer> PostRefreshEscrowContainer(int containerId);

        Task<PostEscrowResolveResponse> PostResolveEscrowContainer(int containerId, List<EscrowAction> escrowActions);

        Task<EscrowContainer> PostRefundEscrowContainer(int containerId);

        Task<EscrowContainer> GetEscrowContainerById(int containerId);

        Task<GetUserProfileResponse> GetUserProfile(string authToken);

        Task<GetUserNFTsResponse> GetUserNFTAssets(string authToken, int page, int pageSize, List<string> categories);

        Task<GetUserBalancesResponse> GetUserBalances(string authToken);

        Task<GetUserPropertyResponse> GetUserProperties(string authToken, int page, int pageSize);

        Task<PostUserJoinEscrowResponse> PostJoinEscrow(string authToken, PostUserJoinEscrow request);
    }
}
