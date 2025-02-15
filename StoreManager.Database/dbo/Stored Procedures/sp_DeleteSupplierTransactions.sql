CREATE Procedure [dbo].[sp_DeleteSupplierTransactions]
	@Id Int,
	@AccountId Int
As
Begin
	set nocount on;

	If dbo.HasPermission(@AccountId, '010009') = 0
	Begin
		Raiserror('This employee doesnt have permission to use this command.', 16, 1)
		Return 1;
	End

	delete from SupplierTransactions 
	where Id = @Id

	Return 0;
End