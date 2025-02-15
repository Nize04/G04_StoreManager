CREATE TABLE [dbo].[OrderDetails] (
    [OrderId]   INT      NOT NULL,
    [ProductId] INT      NOT NULL,
    [Quantity]  SMALLINT NOT NULL,
    [UnitPrice] MONEY    NOT NULL,
    [IsActive]  BIT      CONSTRAINT [DF_OrderDetails_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED ([OrderId] ASC, [ProductId] ASC),
    CONSTRAINT [CK_OrderDetails_quantity] CHECK ([Quantity]>=(0)),
    CONSTRAINT [FK__OrderDeta__Order__595B4002] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]),
    CONSTRAINT [FK__OrderDeta__Produ__5A4F643B] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id])
);

