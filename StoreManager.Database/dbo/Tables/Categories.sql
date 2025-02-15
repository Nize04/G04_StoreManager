CREATE TABLE [dbo].[Categories] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)   NOT NULL,
    [Description] NVARCHAR (1000) NULL,
    [IsActive]    BIT             DEFAULT ((1)) NOT NULL,
    [CreateDate]  DATETIME        DEFAULT (getdate()) NOT NULL,
    [UpdateDate]  DATETIME        NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CHECK (len([Name])>=(2)),
    UNIQUE NONCLUSTERED ([Name] ASC)
);

