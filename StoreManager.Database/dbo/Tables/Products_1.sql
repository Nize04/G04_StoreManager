CREATE TABLE [dbo].[Products] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [CategoryId]  INT             NOT NULL,
    [SupplierId]  INT             NULL,
    [Name]        NVARCHAR (50)   NOT NULL,
    [Description] NVARCHAR (1000) NULL,
    [CreateDate]  DATETIME        CONSTRAINT [DF_Products_CreateDate] DEFAULT (getdate()) NOT NULL,
    [UpdateDate]  DATETIME        NULL,
    [IsActive]    BIT             CONSTRAINT [DF_Products_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Products_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]),
    CONSTRAINT [FK_Products_Suppliers] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers] ([Id])
);

