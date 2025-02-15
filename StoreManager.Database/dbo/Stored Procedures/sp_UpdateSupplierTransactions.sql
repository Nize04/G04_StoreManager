CREATE procedure [dbo].[sp_UpdateSupplierTransactions]
	@Id int,
	@SupplierId Int,
	@TransactionDate Datetime,
	@AccountId Int
As
Begin
	set nocount on;

	If dbo.HasPermission(@AccountId, '030009') = 0
	Begin
		Raiserror('This employee doesnt have permission to use this command.', 16, 1)
		Return 1;
	End

	Update SupplierTransactions
	Set TransactionDate = @TransactionDate,SupplierId = @SupplierId
	Where Id = @Id

	Return 0;
End