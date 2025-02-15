CREATE PROCEDURE [dbo].[sp_AuthenticateUser]
    @Email NVARCHAR(100),
    @Password NVARCHAR(100),
    @IsAuthenticate BIT OUTPUT
AS
BEGIN
    DECLARE @StoredPasswordHash NVARCHAR(100);
    DECLARE @Salt NVARCHAR(100);
    DECLARE @ComputedHash NVARCHAR(100);
	DECLARE @IsActive BIT;

    SELECT @StoredPasswordHash = Password, @Salt = Salt, @IsActive = IsActive 
    FROM Accounts
    WHERE Email = @Email

	IF (@IsActive = 0)
	BEGIN
		SET @IsAuthenticate = 0;
		RAISERROR('user with this email doesnt exists',16, 3);
		RETURN 3;
	END

    IF (@StoredPasswordHash IS NOT NULL AND @Salt IS NOT NULL)
    BEGIN
        SET @ComputedHash = dbo.HashPassword(@Password, @Salt);

        IF (@ComputedHash = @StoredPasswordHash)
        BEGIN
			SET @IsAuthenticate = 1;
			RETURN 0;
        END
        ELSE
        BEGIN
            RAISERROR('Password is incorrect',16, 2);
			RETURN 2;
        END
    END
    ELSE
    BEGIN
        RAISERROR('User with this email not found',16, 1)
		RETURN 1;
    END

	return 0;
END