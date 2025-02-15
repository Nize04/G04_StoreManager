CREATE function [dbo].[GetProductPrice](@Id INT)
	RETURNS MONEY
AS
BEGIN
	DECLARE @Price MONEY;

	SELECT TOP 1 @Price = Price 
	FROM PriceHistory
	WHERE ProductId = @Id
	ORDER BY CreateDate DESC

	RETURN @Price;
END