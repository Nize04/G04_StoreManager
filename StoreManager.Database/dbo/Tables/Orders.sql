CREATE TABLE [dbo].[Orders] (
    [Id]         INT      IDENTITY (1, 1) NOT NULL,
    [CustomerId] INT      NULL,
    [EmployeeId] INT      NOT NULL,
    [CreateDate] DATETIME CONSTRAINT [DF_Orders_OrderDate] DEFAULT (getdate()) NOT NULL,
    [IsActive]   BIT      CONSTRAINT [DF_Orders_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK__Orders__3214EC07E1D178C8] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Orders_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);

