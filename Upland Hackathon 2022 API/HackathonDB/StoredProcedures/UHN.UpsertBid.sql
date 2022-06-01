CREATE PROCEDURE [UHN].[UpsertBid]
(
	@Id                       INT                       ,
	@ContainerId			  INT                       ,
	@UserId                   INT                       ,
	@AuctionId                INT                       ,
	@BidDateTime              DATETIME                  ,
	@Amount                   INT                       ,
	@Winner                   BIT                       ,
	@Fee                      INT                       
)
AS
BEGIN
	BEGIN TRY	
		IF(NOT EXISTS(SELECT * FROM [UHN].[Bid] (NOLOCK) WHERE [Id] = @Id))
			BEGIN
				INSERT INTO [UHN].[Bid]
				(
					[ContainerId],
					[UserId],
					[AuctionId],
					[BidDateTime],
					[Amount],
					[Winner],
					[Fee]
				)
				Values
				(
					@ContainerId,
					@UserId,
					@AuctionId,
					@BidDateTime,
					@Amount,
					@Winner,
					@Fee               
				)
			END
		ELSE
			BEGIN
				UPDATE [UHN].[Bid]
				SET
					[ContainerId] = @ContainerId,
					[UserId] = @UserId,
					[AuctionId] = @AuctionId,
					[BidDateTime] = @BidDateTime,
					[Amount] = @Amount,
					[Winner] = @Winner,
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