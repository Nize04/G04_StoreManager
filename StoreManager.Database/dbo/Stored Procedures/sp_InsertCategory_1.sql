CREATE PROCEDURE [dbo].[sp_InsertCategory]
	@Name NVARCHAR(50),
	@AccountId INT,
	@Id INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'020001') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command.', 16, 1)
		RETURN 1;
	END

	INSERT INTO Categories (Name)
	VALUES (@Name);

	SET @Id = @@IDENTITY
	RETURN 0;
END;