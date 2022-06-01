CREATE TABLE [UHN].[User]
(
	[Id]                       INT IDENTITY(1,1) NOT NULL,
	[UplandUsername]           VARCHAR(200)      NOT NULL,
	[UplandId]                 UNIQUEIDENTIFIER          ,
	[PasswordSalt]             VARCHAR(64)               ,
	[PasswordHash]             VARCHAR(64)               ,
	[UplandAccessToken]        VARCHAR(500)              ,
	[AccessCode]               VARCHAR(50)               ,

	CONSTRAINT pk_UserId PRIMARY KEY(Id)
)
GO