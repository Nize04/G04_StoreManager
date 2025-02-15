CREATE PROCEDURE [dbo].[sp_GetCustomer]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Customers
    WHERE Id = @Id and IsActive = 1

    RETURN 0;
END
GO