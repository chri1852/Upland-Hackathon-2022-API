CREATE TABLE [UHN].[Auction]
(
	[Id]                       INT IDENTITY(1,1) NOT NULL,
	[ContainerId]			   INT               NOT NULL,
	[UserId]                   INT                       ,
	[AuctionEndDate]           DATETIME                  ,
	[ContainerExpirationDate]  DATETIME                  ,
	[Spark]                    DECIMAL(5, 2)             ,
	[Reserve]                  INT                       ,
	[Ended]                    BIT                       ,
	[Fee]                      INT                       ,

	CONSTRAINT pk_AuctionId PRIMARY KEY(Id),
	CONSTRAINT FK_AuctionUser FOREIGN KEY ([UserId]) REFERENCES [UHN].[User](Id)
)
GO