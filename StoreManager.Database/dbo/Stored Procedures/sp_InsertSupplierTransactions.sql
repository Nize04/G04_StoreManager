CREATE Procedure [dbo].[sp_InsertSupplierTransactions]
	@SupplierId Int,
	@TransactionDate Datetime,
	@AccountId Int,
	@Id INT OUTPUT
As
Begin
	set nocount on;

	if dbo.HasPermission(@AccountId, '020009') = 0
	Begin
		Raiserror('This employee doesnt have permission to use this command.', 16, 1)
		Return 1;
	End

	Insert into SupplierTransaction (SupplierId, TransactionDate)
	Values(@SupplierId, @TransactionDate)

	SET @Id = @@IDENTITY

	return 0;
End