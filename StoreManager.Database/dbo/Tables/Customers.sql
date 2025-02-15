CREATE TABLE [dbo].[Customers] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [FullName]    NVARCHAR (60) NOT NULL,
    [PhoneNumber] VARCHAR (12)  NOT NULL,
    [CreateDate]  DATETIME      CONSTRAINT [DF_Customers_CreateDate] DEFAULT (getdate()) NOT NULL,
    [UpdateDate]  DATETIME      NULL,
    [IsActive]    BIT           CONSTRAINT [DF_Customers_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

