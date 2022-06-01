CREATE TABLE [UHN].[AuctionAsset]
(
	[Id]                       INT IDENTITY(1,1) NOT NULL,
	[AuctionId]		    	   INT                       ,
	[AssetId]                  INT                       ,
	[AssetCategory]            VARCHAR(50)               ,
	[AssetName]                VARCHAR(2000)             ,
	[Thumbnail]                VARCHAR(2000)

	CONSTRAINT pk_AuctionAssetId PRIMARY KEY(Id),
	CONSTRAINT FK_AuctionAssetAuction FOREIGN KEY ([AuctionId]) REFERENCES [UHN].[Auction](Id)
)
GO