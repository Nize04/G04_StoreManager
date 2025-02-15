CREATE TABLE [dbo].[Permissions] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [PermissionCode] VARCHAR (6)   NOT NULL,
    [PermissionName] NVARCHAR (50) NOT NULL,
    [CreateDate]     DATETIME      CONSTRAINT [DF_Permissions_CreateDate] DEFAULT (getdate()) NOT NULL,
    [IsActive]       BIT           CONSTRAINT [DF_Permissions_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_PersmissionCode] UNIQUE NONCLUSTERED ([Id] ASC)
);

