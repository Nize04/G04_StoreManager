CREATE PROCEDURE [dbo].[sp_DeleteCategory]
	@Id INT,
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'010001') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command.', 16, 1)
		RETURN 1;
	END

	BEGIN TRY
	BEGIN TRANSACTION;
		UPDATE Categories SET IsActive = 0
			WHERE Id = @Id AND IsActive = 1;

		UPDATE Products SET IsActive = 0
		    WHERE CategoryId = @Id AND IsActive = 1;
    COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
	END CATCH;

	RETURN 0;
END