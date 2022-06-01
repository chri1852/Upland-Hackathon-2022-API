CREATE TABLE [UHN].[Bid]
(
	[Id]                       INT IDENTITY(1,1) NOT NULL,
	[ContainerId]			   INT               NOT NULL,
	[UserId]                   INT                       ,
	[AuctionId]                INT                       ,
	[BidDateTime]              DATETIME                  ,
	[Amount]                   INT                       ,
	[Winner]                   BIT                       ,
	[Fee]                      INT                       ,

	CONSTRAINT pk_BidId PRIMARY KEY(Id),
	CONSTRAINT FK_BidAuction FOREIGN KEY ([AuctionId]) REFERENCES [UHN].[Auction](Id),
	CONSTRAINT FK_BidUser FOREIGN KEY ([UserId]) REFERENCES [UHN].[User](Id)
)
GO