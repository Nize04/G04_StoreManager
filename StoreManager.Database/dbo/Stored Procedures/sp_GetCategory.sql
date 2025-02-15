CREATE PROCEDURE [dbo].[sp_GetCategory]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Categories
    WHERE Id = @Id and IsActive = 1

    RETURN 0;
END
GO