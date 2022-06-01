using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.Messages;
using UplandHackathon2022.API.Contracts.Types;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes;
using UplandHackaton2022.Api.Abstractions;

namespace Upland_Hackathon_2022_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IUplandThirdPartyApiRepository _uplandThirdPartyApiRepository;
        private readonly ILocalRepository _localRepository;

        public AuctionController(IUplandThirdPartyApiRepository thirdPartyRepo, ILocalRepository localRepo)
        {
            _uplandThirdPartyApiRepository = thirdPartyRepo;
            _localRepository = localRepo;
        }

        [HttpGet("AuctionTest")]
        [AllowAnonymous]
        public async Task<ActionResult<GenericResponse>> GetTest()
        {
            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername("HB3_DEV");

            //GetUserPropertyResponse propertyResponse = await _uplandThirdPartyApiRepository.GetUserProperties(registeredUser.UplandAccessToken, 1, 100);
            //GetUserProfileResponse profile = await _uplandThirdPartyApiRepository.GetUserProfile(registeredUser.UplandAccessToken);
            GetUserBalancesResponse balances = await _uplandThirdPartyApiRepository.GetUserBalances(registeredUser.UplandAccessToken);
            /*
            GetUserNFTsResponse nfts = await _uplandThirdPartyApiRepository.GetUserNFTAssets(registeredUser.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            });
            */
            EscrowContainer auctionContainer = await _uplandThirdPartyApiRepository.PostNewEscrowContainer(6);
            PostUserJoinEscrowResponse escrowJoinResponse = await _uplandThirdPartyApiRepository.PostJoinEscrow(registeredUser.UplandAccessToken, new PostUserJoinEscrow
            {
                containerId = auctionContainer.id,
                upxAmount = 2000,
                sparkAmount = 0,
                assets = new List<NFTAsset>()
            });

            // Create a container for the auction
            //EscrowContainer auctionContainer = await _uplandThirdPartyApiRepository.PostNewEscrowContainer(6);
            /*
            Auction auction = new Auction();

            auction.Id = -1;
            auction.UserId = registeredUser.Id;
            auction.UplandUsername = registeredUser.UplandUsername;
            auction.Fee = 100;
            auction.Ended = false;
            auction.AuctionEndDate = System.DateTime.Now.AddDays(7);
            auction.Bids = new List<Bid>();
            auction.Assets = new List<AuctionAsset>();
            auction.Spark = 1;
            auction.Reserve = 10000;
            auction.ContainerId = auctionContainer.id;
            auction.ContainerExpirationDate = auctionContainer.expirationDate;

            // Lets Create the Auction
            _localRepository.UpsertAuction(auction);

            int newAuctionId = _localRepository.GetAuctionIdByContainerId(auctionContainer.id);
            auction.Id = newAuctionId;

            auction.Assets.Add(new AuctionAsset
            {
                Id = -1,
                AssetCategory = nfts.results[0].category,
                AssetName = nfts.results[0].name,
                AuctionId = newAuctionId,
                AssetId = nfts.results[0].id,
                Thumbnail = nfts.results[0].thumbnail
            });

            auction.Assets.Add(new AuctionAsset
            {
                Id = -1,
                AssetCategory = nfts.results[2].category,
                AssetName = nfts.results[2].name,
                AuctionId = newAuctionId,
                AssetId = nfts.results[2].id,
                Thumbnail = nfts.results[2].thumbnail
            });
            
            foreach (AuctionAsset asset in auction.Assets)
            {
                _localRepository.UpsertAuctionAsset(asset);
            }


            PostUserJoinEscrowResponse escrowJoinResponse = await _uplandThirdPartyApiRepository.PostJoinEscrow(registeredUser.UplandAccessToken, new PostUserJoinEscrow
            {
                containerId = auction.ContainerId,
                upxAmount = 0,
                sparkAmount = (double)auction.Spark,
                assets = auction.Assets.Select(a => new NFTAsset { id = a.AssetId, category = a.AssetCategory }).ToList()
            });
            */
            // At this point the auction is created and the container is set

            // Lets refund it
            await _uplandThirdPartyApiRepository.PostRefundEscrowContainer(auctionContainer.id);

            return Ok(new GenericResponse { Message = "Success" });
        }
    }
}
