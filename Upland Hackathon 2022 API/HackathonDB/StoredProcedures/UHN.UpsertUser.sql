CREATE PROCEDURE [UHN].[UpsertUser]
(
	@UplandUsername           VARCHAR(200),
	@UplandId                 UNIQUEIDENTIFIER,
	@PasswordSalt             VARCHAR(64),
	@PasswordHash             VARCHAR(64),
	@UplandAccessToken        VARCHAR(500),
	@AccessCode               VARCHAR(50)
)
AS
BEGIN
	BEGIN TRY	
		IF(NOT EXISTS(SELECT * FROM [UHN].[User] (NOLOCK) WHERE [UplandUsername] = @UplandUsername))
			BEGIN
				INSERT INTO [UHN].[User]
				(
					[UplandUsername],
					[UplandId],
					[PasswordSalt],
					[PasswordHash],
					[UplandAccessToken],
					[AccessCode]
				)
				Values
				(
					@UplandUsername,
					@UplandId,
					@PasswordSalt,
					@PasswordHash,
					@UplandAccessToken,
					@AccessCode
				)
			END
		ELSE
			BEGIN
				UPDATE [UHN].[User]
				SET
					[UplandId] = @UplandId,
					[PasswordSalt] = @PasswordSalt,
					[PasswordHash] = @PasswordHash,
					[UplandAccessToken] = @UplandAccessToken,
					[AccessCode] = @AccessCode
				WHERE [UplandUsername] = @UplandUsername
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