CREATE PROCEDURE [dbo].[sp_InsertAccount]
	@Id INT,
    @Email NVARCHAR(50),
    @Password NVARCHAR(100),
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'020003') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command.', 16, 1)
		RETURN 1;
	END

	IF(dbo.CheckPasswordRequirements(@Password) = 0)
	BEGIN
		RAISERROR('Password does not meet requirements', 16, 2);
        RETURN 2;
	END

	DECLARE @PasswordHash NVARCHAR(150)
    DECLARE @Salt NVARCHAR(100) = NEWID();
    SET @PasswordHash = dbo.HashPassword(@Password,@Salt)

    INSERT INTO Accounts(Id,Password,Salt)
    VALUES (@Id, @Email,@PasswordHash,@Salt);

	RETURN 0;
END;