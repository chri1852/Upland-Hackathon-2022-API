CREATE PROCEDURE [UHN].[UpsertAuction]
(
	@Id                       INT                       ,
	@ContainerId			  INT                       ,
	@UserId                   INT                       ,
	@AuctionEndDate           DATETIME                  ,
	@ContainerExpirationDate  DATETIME                  ,
	@Spark                    DECIMAL(5, 2)             ,
	@Reserve                  INT                       ,
	@Ended                    BIT                       ,
	@Fee                      INT                       

)
AS
BEGIN
	BEGIN TRY	
		IF(NOT EXISTS(SELECT * FROM [UHN].[Auction] (NOLOCK) WHERE [Id] = @Id))
			BEGIN
				INSERT INTO [UHN].[Auction]
				(
					[ContainerId],
					[UserId],
					[AuctionEndDate],
					[ContainerExpirationDate],
					[Spark],
					[Reserve],
					[Ended],
					[Fee]
				)
				Values
				(
					@ContainerId,
					@UserId,
					@AuctionEndDate,
					@ContainerExpirationDate,
					@Spark,
					@Reserve,
					@Ended,
					@Fee               
				)
			END
		ELSE
			BEGIN
				UPDATE [UHN].[Auction]
				SET
					[ContainerId] = @ContainerId,
					[UserId] = @UserId,
					[AuctionEndDate] = @AuctionEndDate,
					[ContainerExpirationDate] = @ContainerExpirationDate,
					[Spark] = @Spark,
					[Reserve] = @Reserve,
					[Ended] = @Ended,
					[Fee] = @Fee
				WHERE [Id] = @Id
			END
	END TRY

	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT @ErrorMessage = ERROR_MESSAGE(),
			   @ErrorSeverity = ERROR_SEVERITY(),
			   @ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	END CATCH
END
GO