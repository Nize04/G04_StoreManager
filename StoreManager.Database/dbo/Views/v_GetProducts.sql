CREATE VIEW v_GetProducts
AS
SELECT 
  P.Id,
  P.CategoryId,
  P.SupplierId,
  P.Name,
  P.CreateDate,
  P.UpdateDate,
  (SELECT TOP 1 Price 
   FROM PriceHistory
   WHERE ProductId = Id 
   ORDER BY CreateDate DESC) AS Price
FROM Products P
WHERE IsActive = 1;
