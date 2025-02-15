CREATE TABLE [dbo].[SupplierTransactionDetails] (
    [SupplierTransactionId] BIGINT     NOT NULL,
    [ProductId]             INT        NOT NULL,
    [Price]                 MONEY      NOT NULL,
    [Quantity]              NCHAR (10) NOT NULL,
    [ProductionDate]        DATETIME   NOT NULL,
    [ExpirationDate]        DATETIME   NULL,
    [IsActive]              BIT        CONSTRAINT [DF_SupplierTransactionDetails_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_SupplierTransactionDetails] PRIMARY KEY CLUSTERED ([SupplierTransactionId] ASC, [ProductId] ASC),
    CONSTRAINT [FK_SupplierTransactionDetails_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]),
    CONSTRAINT [FK_SupplierTransactionDetails_SupplierTransactions] FOREIGN KEY ([SupplierTransactionId]) REFERENCES [dbo].[SupplierTransactions] ([Id])
);

