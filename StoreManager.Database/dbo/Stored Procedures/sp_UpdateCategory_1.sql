CREATE PROCEDURE [dbo].[sp_UpdateCategory]
   @ID int,
   @Name nvarchar(50),
   @AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'030001') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command.', 16, 1)
		RETURN 1;
	END

	UPDATE Categories
	SET Name = @Name,
		UpdateDate = GETDATE()
	WHERE ID = @ID;

  RETURN 0;
END;