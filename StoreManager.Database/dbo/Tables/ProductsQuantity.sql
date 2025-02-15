CREATE TABLE [dbo].[ProductsQuantity] (
    [Id]         INT      NOT NULL,
    [Quantity]   INT      CONSTRAINT [DF_ProductsQuantity_Quantity] DEFAULT ((0)) NOT NULL,
    [UpdateDate] DATETIME NULL,
    CONSTRAINT [PK_ProductsQuantity] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_ProductsQuantity_quantity] CHECK ([Quantity]>=(0)),
    CONSTRAINT [FK_ProductsQuantity_Products] FOREIGN KEY ([Id]) REFERENCES [dbo].[Products] ([Id])
);

