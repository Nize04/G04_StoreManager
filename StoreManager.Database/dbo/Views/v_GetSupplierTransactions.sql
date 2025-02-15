Create view [dbo].[v_GetSupplierTransactions]
As
	Select
	st.Id,
	st.SupplierId,
	st.TransactionDate
From SupplierTransactions st
WHERE IsActive = 1;