CREATE PROCEDURE [UHN].[GetAllOpenAuctionIds]
AS
BEGIN
	BEGIN TRY		
		SELECT A.Id
		FROM UHN.Auction A (NOLOCK)
		WHERE A.Ended = 0
		ORDER BY AuctionEndDate
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
