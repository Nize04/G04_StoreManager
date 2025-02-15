CREATE TABLE [dbo].[SupplierTransactions] (
    [Id]              BIGINT   IDENTITY (1, 1) NOT NULL,
    [SupplierId]      INT      NOT NULL,
    [TransactionDate] DATETIME NOT NULL,
    [IsActive]        BIT      CONSTRAINT [DF_SupplierTransactions_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_SupplierTransactions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SupplierTransactions_Suppliers] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers] ([Id])
);

