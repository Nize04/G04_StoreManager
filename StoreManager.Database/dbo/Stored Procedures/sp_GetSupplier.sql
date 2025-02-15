CREATE PROCEDURE [dbo].[sp_GetSupplier]
    @Id INT
AS
BEGIN 
    SET NOCOUNT ON;

    SELECT *
    FROM Suppliers
    where Id = @Id and IsActive = 1

    RETURN 0;
END
GO