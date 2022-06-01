using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public class BattleController : ControllerBase
    {
        private readonly IUplandThirdPartyApiRepository _thirdPartyApiRepository;
        private readonly ILocalRepository _localRepository;

        private const string SKILL_ROCK = "Rock";
        private const string SKILL_PAPER = "Paper";
        private const string SKILL_SISSORS = "Sissors";
        private const string HB3_DEV_EOS = "zbnhjeesdyy4";
        private const string HB5_DEV_EOS = "u5c4ltgfzdyg";

        private RegisteredUser BattleMaster;

        public BattleController(IUplandThirdPartyApiRepository thirdPartyRepo, ILocalRepository localRepo)
        {
            _thirdPartyApiRepository = thirdPartyRepo;
            _localRepository = localRepo;

            BattleMaster = _localRepository.GetUserByUplandUsername("HB5_DEV");
        }

        [HttpGet("Test")]
        [AllowAnonymous]
        public async Task<ActionResult<GenericResponse>> GetTest()
        {
            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername("HB3_DEV");

            await this.ClearExpiredTrainings();
            await this.CompleteFinishedTrainings();

            //GetUserPropertyResponse propertyResponse = await _uplandThirdPartyApiRepository.GetUserProperties(registeredUser.UplandAccessToken, 1, 100);
            //GetUserProfileResponse profile = await _uplandThirdPartyApiRepository.GetUserProfile(registeredUser.UplandAccessToken);
            GetUserBalancesResponse balances = await _thirdPartyApiRepository.GetUserBalances(registeredUser.UplandAccessToken);

            GetUserNFTsResponse nfts = await _thirdPartyApiRepository.GetUserNFTAssets(registeredUser.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            });

            BattleAsset battleAsset = this.GetBattleAssetFromNFT(nfts.results[0]);

            BattleAssetTraining battleAssetTraining = await this.TrainBattleAsset(registeredUser, battleAsset, balances.availableUpx, SKILL_ROCK, 5);

            // Lets refund it
            await _thirdPartyApiRepository.PostRefundEscrowContainer(battleAssetTraining.ContainerId);

            return Ok(new GenericResponse { Message = "Success" });
        }

        private BattleAsset GetBattleAssetFromNFT(NFTAsset asset)
        {
            BattleAsset battleAsset = _localRepository.GetBattleAssetByAssetId(asset.id);

            // If it is null it is a new asset not in the system
            if (battleAsset == null)
            {
                battleAsset = new BattleAsset();
                battleAsset.AssetId = asset.id;
                battleAsset.AssetCategory = asset.category;
                battleAsset.AssetName = asset.name;
                battleAsset.Thumbnail = asset.thumbnail;
                battleAsset.RockSkill = 50;
                battleAsset.PaperSkill = 50;
                battleAsset.SissorsSkill = 50;

                switch (asset.category)
                {
                    case "blkexplorer":
                        battleAsset.SissorsSkill = 60;
                        break;
                    case "spirithlwn":
                        battleAsset.PaperSkill = 60;
                        break;
                    case "structornmt":
                        battleAsset.SissorsSkill = 60;
                        break;
                }

                _localRepository.UpsertBattleAsset(battleAsset);
                battleAsset = _localRepository.GetBattleAssetByAssetId(asset.id);
            }

            return battleAsset;
        }

        private async Task<BattleAssetTraining> TrainBattleAsset(RegisteredUser registeredUser, BattleAsset battleAsset, int upxBalance, string skill, int timeInHours)
        {
            // Check to make sure it is not actively training
            int currentTrainingId = _localRepository.IsBattleAssetTraining(battleAsset.Id);
            if (currentTrainingId != -1)
            {
                return _localRepository.GetBattleAssetTrainingById(currentTrainingId);
            }

            if (upxBalance < timeInHours * 25)
            {
                throw new Exception("Not Enough Cash On Hand!");
            }

            BattleAssetTraining battleAssetTraining = new BattleAssetTraining();

            battleAssetTraining.Id = -1;
            battleAssetTraining.BattleAssetId = battleAsset.Id;
            battleAssetTraining.ContainerId = -1;
            battleAssetTraining.SkillTraining = skill;
            battleAssetTraining.FinishedTime = DateTime.UtcNow.AddHours(timeInHours);
            battleAssetTraining.MustAcceptBy = DateTime.UtcNow.AddMinutes(10);
            battleAssetTraining.UPXFee = timeInHours * 50;
            battleAssetTraining.Resolved = false;
            battleAssetTraining.Accepted = false;

            NFTAsset trainRequestNft = new NFTAsset
            {
                id = battleAsset.AssetId,
                category = battleAsset.AssetCategory
            };

            PostUserJoinEscrow trainRequest = new PostUserJoinEscrow
            {
                containerId = -1,
                upxAmount = battleAssetTraining.UPXFee,
                sparkAmount = 0,
                assets = new List<NFTAsset>
                {
                    trainRequestNft
                }
            };

            PostUserJoinEscrow battleMasterShare = new PostUserJoinEscrow
            {
                containerId = -1,
                upxAmount = 1,
                sparkAmount = 0,
                assets = new List<NFTAsset>()
            };

            // Everything is setup Let try and save
            try
            {
                // Create the escrow container and make the request
                EscrowContainer trainingContainer = await _thirdPartyApiRepository.PostNewEscrowContainer(24);

                trainRequest.containerId = trainingContainer.id;
                battleMasterShare.containerId = trainingContainer.id;

                PostUserJoinEscrowResponse escrowJoinResponse = await _thirdPartyApiRepository.PostJoinEscrow(registeredUser.UplandAccessToken, trainRequest);
                PostUserJoinEscrowResponse escrowJoinBattleMasterResponse = await _thirdPartyApiRepository.PostJoinEscrow(BattleMaster.UplandAccessToken, battleMasterShare);

                battleAssetTraining.ContainerId = trainingContainer.id;
                _localRepository.UpsertBattleAssetTraining(battleAssetTraining);
            }
            catch
            {
                throw;
            }

            return battleAssetTraining;
        }

        private async Task ClearExpiredTrainings()
        {
            List<BattleAssetTraining> expiredTrainings = _localRepository.GetAllExpiredBattleAssetTrainings();

            foreach (BattleAssetTraining training in expiredTrainings)
            {
                BattleAsset asset = _localRepository.GetBattleAssetByBattleAssetId(training.BattleAssetId);

                List<EscrowAction> escrowActions = new List<EscrowAction>
                {
                    new EscrowAction
                    {
                        assetId = asset.AssetId,
                        category = asset.AssetCategory
                    },
                    new EscrowAction
                    {
                        amount = training.UPXFee,
                        category = "upx"
                    },
                    new EscrowAction
                    {
                        amount = 1,
                        category = "upx",
                        targetEosId = HB5_DEV_EOS
                    }
                };

                try
                {
                    EscrowContainer container = await _thirdPartyApiRepository.GetEscrowContainerById(training.ContainerId);

                    if (container.assets.Any(a => a.status == "user_signature_requested"))
                    {
                        escrowActions[0].targetEosId = container.assets.First(a => a.category == asset.AssetCategory && a.assetId == asset.AssetId).ownerEosId;
                        escrowActions[1].targetEosId = escrowActions[0].targetEosId;
                        training.Resolved = true;

                        await _thirdPartyApiRepository.PostResolveEscrowContainer(training.ContainerId, escrowActions);

                        _localRepository.UpsertBattleAssetTraining(training);
                    }
                    else
                    {
                        // The User has accepted
                        training.Accepted = true;
                        _localRepository.UpsertBattleAssetTraining(training);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private async Task CompleteFinishedTrainings()
        {
            List<BattleAssetTraining> finishedTrainings = _localRepository.GetAllFinishedBattleAssetTrainings();

            foreach (BattleAssetTraining training in finishedTrainings)
            {
                training.Resolved = true;
                BattleAsset asset = _localRepository.GetBattleAssetByBattleAssetId(training.BattleAssetId);

                List<EscrowAction> escrowActions = new List<EscrowAction>
                {
                    new EscrowAction
                    {
                        assetId = asset.AssetId,
                        category = asset.AssetCategory
                    },
                    new EscrowAction
                    {
                        amount = training.UPXFee + 1,
                        category = "upx",
                        targetEosId = HB5_DEV_EOS
                    }
                };

                try
                {
                    EscrowContainer container = await _thirdPartyApiRepository.GetEscrowContainerById(training.ContainerId);
                    escrowActions[0].targetEosId = container.assets.First(a => a.category == asset.AssetCategory && a.assetId == asset.AssetId).ownerEosId;

                    await _thirdPartyApiRepository.PostResolveEscrowContainer(training.ContainerId, escrowActions);

                    _localRepository.UpsertBattleAssetTraining(training);

                    asset = this.UpdateSkillAfterTraining(asset, training);
                    _localRepository.UpsertBattleAsset(asset);
                }
                catch
                {
                    throw;
                }
            }
        }

        private BattleAsset UpdateSkillAfterTraining(BattleAsset asset, BattleAssetTraining training)
        {
            int addToSkill = (int)Math.Round((training.FinishedTime - training.MustAcceptBy.AddMinutes(-10)).TotalHours);

            switch (training.SkillTraining)
            {
                case SKILL_ROCK:
                    asset.RockSkill += addToSkill;
                    break;
                case SKILL_PAPER:
                    asset.PaperSkill += addToSkill;
                    break;
                case SKILL_SISSORS:
                    asset.SissorsSkill += addToSkill;
                    break;
            }

            return asset;
        }

    }
}
