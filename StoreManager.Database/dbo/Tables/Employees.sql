CREATE TABLE [dbo].[Employees] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]  NVARCHAR (50) NOT NULL,
    [LastName]   NVARCHAR (50) NOT NULL,
    [ReportsTo]  INT           NULL,
    [CreateDate] DATETIME      CONSTRAINT [DF_Employees_CreateDate] DEFAULT (getdate()) NOT NULL,
    [UpdateDate] DATETIME      NULL,
    [IsActive]   BIT           CONSTRAINT [DF_Employees_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Employees_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Employees_Employees] FOREIGN KEY ([ReportsTo]) REFERENCES [dbo].[Employees] ([Id])
);

