CREATE VIEW v_GetCategories AS
SELECT *
FROM Categories
WHERE IsActive = 1;