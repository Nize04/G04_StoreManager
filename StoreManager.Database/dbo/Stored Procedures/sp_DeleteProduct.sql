CREATE PROCEDURE [dbo].[sp_DeleteProduct]
	@Id INT,
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId, '010002') = 0
	BEGIN
		RAISERROR('This employee doesnt have permission to use this command.', 16, 1)
		RETURN 1;
	END

	UPDATE Products SET IsActive = 0
		WHERE Id = @Id AND IsActive = 1

	RETURN 0;
END