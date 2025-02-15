CREATE FUNCTION [dbo].[CheckPasswordRequirements](@Password NVARCHAR(100))
RETURNS BIT
AS
BEGIN
    IF LEN(@Password) < 8 OR  @Password NOT LIKE '%[0-9]%'
        RETURN 0;

	RETURN 1;
END