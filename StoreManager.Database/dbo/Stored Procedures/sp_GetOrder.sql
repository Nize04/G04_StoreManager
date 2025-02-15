CREATE PROCEDURE [dbo].[sp_GetOrder]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Orders O
    WHERE Id = @Id

    RETURN 0;
END
GO