CREATE PROCEDURE [dbo].[sp_GetProduct]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *, (SELECT TOP 1 Price 
    FROM PriceHistory
    WHERE ProductId = Id 
    ORDER BY CreateDate DESC) AS Price 
    FROM Products
    WHERE Id = @Id and IsActive = 1

    RETURN 0;
END
GO