using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.Constants;
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

        
        [HttpGet("TestBattle")]
        [AllowAnonymous]
        public async Task<ActionResult> TestBattle()
        {
            /*
            RegisteredUser opponent = _localRepository.GetUserByUplandUsername("HB3_DEV");
            RegisteredUser challenger = _localRepository.GetUserByUplandUsername("HB5_DEV");
            List<BattleAsset> opponentBattleAssets = (await _thirdPartyApiRepository.GetUserNFTAssets(opponent.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            })).results.Select(n => this.GetBattleAssetFromNFT(n)).ToList();
            List<BattleAsset> challengerBattleAssets = (await _thirdPartyApiRepository.GetUserNFTAssets(challenger.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            })).results.Select(n => this.GetBattleAssetFromNFT(n)).ToList();
            GetUserBalancesResponse opponentBalances = await _thirdPartyApiRepository.GetUserBalances(opponent.UplandAccessToken);
            GetUserBalancesResponse challengerBalances = await _thirdPartyApiRepository.GetUserBalances(challenger.UplandAccessToken);

            await this.CreateBattle(opponent, opponentBattleAssets[1], 500, opponentBalances.availableUpx);

            List<Battle> activeBattles = _localRepository.GetAllNeedingChallengers();

            await this.JoinBattle(challenger, challengerBattleAssets[0], challengerBalances.availableUpx, activeBattles[0].Id);

            await UpdateBattles();
            */
            await UpdateBattles();
            return Ok();
        }
        

        [HttpGet("Profile")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public async Task<ActionResult<GetUIUserProfileResponse>> GetUserProfile()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || identity.FindFirst("UplandUsername") == null)
            {
                return BadRequest("Bad Token");
            }

            try
            {
                RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(identity.FindFirst("UplandUsername").Value);
                GetUIUserProfileResponse response = new GetUIUserProfileResponse();
                response.userProfile = await _thirdPartyApiRepository.GetUserProfile(registeredUser.UplandAccessToken);
                response.userProfile.battleAssets = new List<BattleAsset>();
                GetUserNFTsResponse nfts = await _thirdPartyApiRepository.GetUserNFTAssets(registeredUser.UplandAccessToken, 1, 100, new List<string>
                {
                    "blkexplorer",
                    "structornmt",
                    "spirithlwn"
                });

                foreach (NFTAsset asset in nfts.results)
                {
                    response.userProfile.battleAssets.Add(this.GetBattleAssetFromNFT(asset));
                }


                response.Message = "Success";

                return Ok(response);
            }
            catch
            {
                return BadRequest("Could Not Load User");
            }
        }

        [HttpGet("BattleAssets")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public async Task<ActionResult<List<BattleAsset>>> GetBattleAssetsByUser()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || identity.FindFirst("UplandUsername") == null)
            {
                return BadRequest("Bad Token");
            }

            List<BattleAsset> battleAssets = new List<BattleAsset>();

            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(identity.FindFirst("UplandUsername").Value);
            GetUserNFTsResponse nfts = await _thirdPartyApiRepository.GetUserNFTAssets(registeredUser.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            });

            foreach (NFTAsset asset in nfts.results)
            {
                battleAssets.Add(this.GetBattleAssetFromNFT(asset));
            }

            return battleAssets;
        }

        [HttpPost("BattleAssets/Train")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public async Task<ActionResult<BattleAssetTraining>> PostTrainBattleAsset(PostTrainBattleAssetRequest request)
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || identity.FindFirst("UplandUsername") == null)
            {
                return BadRequest("Bad Token");
            }

            List<BattleAsset> battleAssets = new List<BattleAsset>();

            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(identity.FindFirst("UplandUsername").Value);
            GetUserBalancesResponse balances = await _thirdPartyApiRepository.GetUserBalances(registeredUser.UplandAccessToken);
            GetUserNFTsResponse nfts = await _thirdPartyApiRepository.GetUserNFTAssets(registeredUser.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            });

            if (!nfts.results.Any(n => n.id == request.BattleAsset.AssetId))
            {
                return BadRequest("You Don't Own This NFT");
            }
            
            try
            {
                return Ok(await this.TrainBattleAsset(registeredUser, request.BattleAsset, balances.availableUpx, request.TrainSkill, request.TimeInHours));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Debug/CompleteTrainings")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public async Task<ActionResult> DebugCompleteTrainings()
        {
            await ClearExpiredTrainings();
            List<BattleAssetTraining> finishedTrainings = _localRepository.GetAllApprovedNotFinishedBattleAssetTrainings();

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

                    if (container.assets.Any(a => a.status != "in_escrow"))
                    {
                        continue;
                    }

                    escrowActions[0].targetEosId = container.assets.First(a => a.category == asset.AssetCategory && a.assetId == asset.AssetId).ownerEosId;

                    await _thirdPartyApiRepository.PostResolveEscrowContainer(training.ContainerId, escrowActions);

                    _localRepository.UpsertBattleAssetTraining(training);

                    asset = this.UpdateSkillAfterTraining(asset, training);
                    _localRepository.UpsertBattleAsset(asset);
                }
                catch
                {
                    return BadRequest();
                }
            }
            return Ok();
        }

        [HttpPost("Debug/UpdateBattles")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public async Task<ActionResult> DebugUpdateBattles()
        {
            try
            {
                await UpdateBattles();
            }
            catch
            {
                return BadRequest("Updating Battles Failed");
            }

            return Ok();
        }

        [HttpGet("Active")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public ActionResult<List<Battle>> GetBattlesNeedingChallengers()
        {
            return Ok(_localRepository.GetAllNeedingChallengers());
        }

        [HttpPost("Create")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public async Task<ActionResult> CreateBattle(PostCreateBattleRequest request)
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || identity.FindFirst("UplandUsername") == null)
            {
                return BadRequest("Bad Token");
            }

            List<BattleAsset> battleAssets = new List<BattleAsset>();

            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(identity.FindFirst("UplandUsername").Value);
            GetUserBalancesResponse balances = await _thirdPartyApiRepository.GetUserBalances(registeredUser.UplandAccessToken);
            GetUserNFTsResponse nfts = await _thirdPartyApiRepository.GetUserNFTAssets(registeredUser.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            });

            if (!nfts.results.Any(n => n.id == request.BattleAsset.AssetId))
            {
                return BadRequest("You Don't Own This NFT");
            }

            try
            {
                await this.CreateBattle(registeredUser, request.BattleAsset, request.UPXWager, balances.availableUpx);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Join")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public async Task<ActionResult> JoinBattle(PostJoinBattleRequest request)
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || identity.FindFirst("UplandUsername") == null)
            {
                return BadRequest("Bad Token");
            }

            List<BattleAsset> battleAssets = new List<BattleAsset>();

            RegisteredUser registeredUser = _localRepository.GetUserByUplandUsername(identity.FindFirst("UplandUsername").Value);
            GetUserBalancesResponse balances = await _thirdPartyApiRepository.GetUserBalances(registeredUser.UplandAccessToken);
            GetUserNFTsResponse nfts = await _thirdPartyApiRepository.GetUserNFTAssets(registeredUser.UplandAccessToken, 1, 100, new List<string>
            {
                "blkexplorer",
                "structornmt",
                "spirithlwn"
            });

            if (!nfts.results.Any(n => n.id == request.BattleAsset.AssetId))
            {
                return BadRequest("You Don't Own This NFT");
            }

            try
            {
                await this.JoinBattle(registeredUser, request.BattleAsset, balances.availableUpx, request.BattleId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("History/{battleAssetId}")]
        [Authorize(AuthenticationSchemes = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme)]
        [Authorize(Policy = "UplandUsername")]
        public ActionResult<List<Battle>> GetBattleHistory(int battleAssetId)
        {
            List<Battle> battleHistory = _localRepository.GetAllBattlesResolvedByBattleAssetId(battleAssetId);

            return Ok(battleHistory);
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

            // Check if it is actively training
            int currentTrainingId = _localRepository.IsBattleAssetTraining(battleAsset.Id);
            if (currentTrainingId != -1)
            {
                battleAsset.IsTraining = true;
            }
            else
            {
                battleAsset.IsTraining = false;
            }

            // Check if it is actively training
            battleAsset.IsBattling = _localRepository.IsBattling(battleAsset.Id);

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
            battleAssetTraining.MustAcceptBy = DateTime.UtcNow.AddMinutes(0);
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
                throw new Exception("Failed Initializing Training");
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

                    if (container.assets.Any(a => a.status == "user_signature_requested" && a.ownerEosId != HB5_DEV_EOS))
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

        private async Task CreateBattle(RegisteredUser registeredUser, BattleAsset battleAsset, int upxToWager, int availableUpx)
        {
            // Check to make sure it is not actively training
            int currentTrainingId = _localRepository.IsBattleAssetTraining(battleAsset.Id);
            if (currentTrainingId != -1)
            {
                throw new Exception("Asset Training");
            }

            if (availableUpx < upxToWager)
            {
                throw new Exception("Not Enough Cash On Hand!");
            }

            Battle battle = new Battle();

            battle.OpponentBattleAssetId = battleAsset.Id;
            battle.ChallengerBattleAssetId = null;
            battle.UPXPerSide = upxToWager;
            battle.MustBattleBy = DateTime.UtcNow.AddHours(1);
            battle.Resolved = false;
            battle.WinnerBattleAssetId = null;
            battle.OpponentSkills = string.Format("{0} / {1} / {2}", battleAsset.RockSkill, battleAsset.PaperSkill, battleAsset.SissorsSkill);
            battle.ChallengerSkills = null;

            NFTAsset trainRequestNft = new NFTAsset
            {
                id = battleAsset.AssetId,
                category = battleAsset.AssetCategory
            };

            PostUserJoinEscrow trainRequest = new PostUserJoinEscrow
            {
                containerId = -1,
                upxAmount = upxToWager,
                sparkAmount = 0,
                assets = new List<NFTAsset>
                {
                    trainRequestNft
                }
            };

            // Everything is setup Let try and save
            try
            {
                // Create the escrow container and make the request
                EscrowContainer battleContainer = await _thirdPartyApiRepository.PostNewEscrowContainer(2);

                trainRequest.containerId = battleContainer.id;
                battle.ContainerId = battleContainer.id;

                PostUserJoinEscrowResponse escrowJoinResponse = await _thirdPartyApiRepository.PostJoinEscrow(registeredUser.UplandAccessToken, trainRequest);

                _localRepository.UpsertBattle(battle);
            }
            catch
            {
                throw new Exception("Failed Initializing Battle");
            }
        }

        private async Task JoinBattle(RegisteredUser registeredUser, BattleAsset battleAsset, int availableUpx, int battleId)
        {
            // Check to make sure it is not actively training
            int currentTrainingId = _localRepository.IsBattleAssetTraining(battleAsset.Id);
            if (currentTrainingId != -1)
            {
                throw new Exception("Asset Training");
            }

            Battle battle = _localRepository.GetBattleById(battleId);

            if (battle == null)
            {
                throw new Exception("Battle Not Found");
            }

            if (battle.ChallengerBattleAssetId != null)
            {
                throw new Exception("Challenger Already Found");
            }

            if (availableUpx < battle.UPXPerSide)
            {
                throw new Exception("Not Enough Cash On Hand!");
            }

            battle.ChallengerBattleAssetId = battleAsset.Id;
            battle.ChallengerSkills = string.Format("{0} / {1} / {2}", battleAsset.RockSkill, battleAsset.PaperSkill, battleAsset.SissorsSkill);

            NFTAsset trainRequestNft = new NFTAsset
            {
                id = battleAsset.AssetId,
                category = battleAsset.AssetCategory
            };

            PostUserJoinEscrow trainRequest = new PostUserJoinEscrow
            {
                containerId = battle.ContainerId,
                upxAmount = battle.UPXPerSide,
                sparkAmount = 0,
                assets = new List<NFTAsset>
                {
                    trainRequestNft
                }
            };

            // Everything is setup Let try and save
            try
            {
                PostUserJoinEscrowResponse escrowJoinResponse = await _thirdPartyApiRepository.PostJoinEscrow(registeredUser.UplandAccessToken, trainRequest);

                _localRepository.UpsertBattle(battle);
            }
            catch
            {
                throw new Exception("Failed Joining Battle");
            }
        }

        private async Task UpdateBattles()
        {
            List<Battle> unresolvedBattles = _localRepository.GetAllUnresolvedBattles();

            foreach (Battle battle in unresolvedBattles)
            {
                bool hasBattleExpired = battle.MustBattleBy < DateTime.UtcNow;
                try
                {
                    if (battle.ChallengerBattleAssetId == null)
                    {
                        if (hasBattleExpired)
                        {
                            EscrowContainer container = await _thirdPartyApiRepository.GetEscrowContainerById(battle.ContainerId);
                            await this.RefundBattle(battle, container);
                        }
                    }
                    else if (battle.ChallengerBattleAssetId != null)
                    {
                        EscrowContainer container = await _thirdPartyApiRepository.GetEscrowContainerById(battle.ContainerId);
                        // still waiting on approval
                        if (container.assets.Any(a => a.status == "user_signature_requested"))
                        {
                            if (hasBattleExpired)
                            {
                                await this.RefundBattle(battle, container);
                            }
                        }
                        else if (container.assets.All(a => a.status == "in_escrow"))
                        {
                            await this.ResolveBattle(battle, container);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private async Task RefundBattle(Battle battle, EscrowContainer container)
        {
            List<EscrowAction> escrowActions = new List<EscrowAction>();

            foreach (NFTAsset asset in container.assets)
            {   
                if (asset.category == "upx")
                {
                    escrowActions.Add(new EscrowAction
                    {
                        amount = battle.UPXPerSide,
                        category = "upx",
                        targetEosId = asset.ownerEosId
                    });
                }
                else
                {
                    escrowActions.Add(new EscrowAction
                    {
                        assetId = asset.assetId,
                        category = asset.category,
                        targetEosId = asset.ownerEosId
                    });
                }
            }

            battle.Resolved = true;

            await _thirdPartyApiRepository.PostResolveEscrowContainer(battle.ContainerId, escrowActions);

            _localRepository.UpsertBattle(battle);
        }

        private async Task ResolveBattle(Battle battle, EscrowContainer container)
        {
            List<EscrowAction> escrowActions = new List<EscrowAction>();

            battle.Resolved = true;
            battle.WinnerBattleAssetId = this.RunBattle(battle);

            BattleAsset battleAsset = _localRepository.GetBattleAssetByBattleAssetId(battle.WinnerBattleAssetId.Value);
            string WinningEOS = container.assets.First(a => a.assetId == battleAsset.AssetId).ownerEosId;

            foreach (NFTAsset asset in container.assets)
            {
                if (asset.category == "upx")
                {
                    escrowActions.Add(new EscrowAction
                    {
                        amount = battle.UPXPerSide,
                        category = "upx",
                        targetEosId = WinningEOS
                    });
                }
                else
                {
                    escrowActions.Add(new EscrowAction
                    {
                        assetId = asset.assetId,
                        category = asset.category,
                        targetEosId = asset.ownerEosId
                    });
                }
            }

            await _thirdPartyApiRepository.PostResolveEscrowContainer(battle.ContainerId, escrowActions);

            _localRepository.UpsertBattle(battle);
        }

        private int RunBattle(Battle battle)
        {
            return battle.OpponentBattleAssetId;
        }
    }
}
