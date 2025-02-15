Create view [dbo].[v_GetSupplierTransactionDetails]
As
	Select
	std.SupplierTransactionId,
	std.ProductId,
	std.Price,
	std.Quantity,
	std.ProductionDate,
	std.ExpirationDate
From SupplierTransactionDetails std
WHERE IsActive = 1;
