create procedure sp_InsertCategory
	@Name nvarchar(50),
	@Description nvarchar(1000)
as
begin
	set nocount on;

	insert into Categories(Name, Description)
	values(@Name, @Description);

	return 0;
end
