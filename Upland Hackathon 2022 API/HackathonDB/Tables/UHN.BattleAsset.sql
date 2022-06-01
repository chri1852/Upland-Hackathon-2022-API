CREATE TABLE [UHN].[BattleAsset]
(
	[Id]                       INT IDENTITY(1,1) NOT NULL,
	[AssetId]                  INT                       ,
	[AssetCategory]            VARCHAR(50)               ,
	[AssetName]                VARCHAR(2000)             ,
	[Thumbnail]                VARCHAR(2000)             ,
	[RockSkill]                INT                       ,
	[PaperSkill]               INT                       ,
	[SissorsSkill]             INT                       

	CONSTRAINT pk_BattleAssetId PRIMARY KEY(Id)
)
GO