CREATE TABLE [dbo].[PriceHistory] (
    [Id]         INT      IDENTITY (1, 1) NOT NULL,
    [ProductId]  INT      NOT NULL,
    [Price]      MONEY    NOT NULL,
    [CreateDate] DATETIME CONSTRAINT [DF_PriceHistory_CreateDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_ProductsPriceHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProductsPriceHistory_Products] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id])
);

