CREATE PROCEDURE [dbo].[sp_UpdateEmployee]
   @ID int,
   @FirstName nvarchar(50),
   @LastName nvarchar(50),
   @ReportsTo INT,
   @AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'030005') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command.', 16, 1)
		RETURN 1;
	END

	UPDATE Employees
	SET FirstName = @FirstName,
      LastName = @LastName,
	  UpdateDate = GETDATE(),
	  ReportsTo = @ReportsTo
	WHERE ID = @ID;
	
	RETURN 0;
END;