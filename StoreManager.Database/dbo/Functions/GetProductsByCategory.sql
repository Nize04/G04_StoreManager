CREATE FUNCTION [dbo].[GetProductsByCategory](@categoryId INT)
RETURNS TABLE
AS
RETURN
(
    SELECT * FROM v_GetProducts
    WHERE CategoryId = @CategoryId
);