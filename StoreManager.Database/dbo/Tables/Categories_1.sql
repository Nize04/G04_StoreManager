CREATE TABLE [dbo].[Categories] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (50) NOT NULL,
    [CreateDate] DATETIME      CONSTRAINT [DF_Categories_CreateDate] DEFAULT (getdate()) NULL,
    [UpdateDate] DATETIME      NULL,
    [IsActive]   BIT           CONSTRAINT [DF_Categories_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Categories_name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [UQ_Category_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

