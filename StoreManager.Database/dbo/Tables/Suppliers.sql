CREATE TABLE [dbo].[Suppliers] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [CompanyName] NVARCHAR (50) NOT NULL,
    [Country]     NVARCHAR (60) NOT NULL,
    [City]        NVARCHAR (50) NOT NULL,
    [UpdateDate]  DATETIME      NULL,
    [IsActive]    BIT           CONSTRAINT [DF_Suppliers_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

