CREATE PROCEDURE [dbo].[sp_UpdateAccount]
	@Id INT,
	@Email NVARCHAR(50),
	@Password NVARCHAR(100),
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'030003') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command.', 16, 1)
		RETURN 1;
	END

	DECLARE @CurrPasswordHash NVARCHAR(150)
	DECLARE @CurrSalt NVARCHAR(150)

	SELECT @CurrPasswordHash = Password, @CurrSalt = Salt
	FROM Accounts
	WHERE Id = @Id

	IF(@CurrPasswordHash != dbo.HashPassword(@Password,@CurrSalt))
	BEGIN
		IF(dbo.CheckPasswordRequirements(@Password) = 0)
		BEGIN
			RAISERROR('Password does not meet requirements', 16, 2);
			RETURN 2;
		END
		SET @CurrSalt = NEWID()
		SET @CurrPasswordHash = dbo.HashPassword(@Password,@CurrSalt)
	END

	UPDATE Accounts 
	SET Email = @Email, Password = @CurrPasswordHash,
		Salt = @CurrSalt,UpdateDate = GETDATE()
	WHERE Id = @Id

	RETURN 0
END