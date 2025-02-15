CREATE TABLE [dbo].[Roles] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [RoleName]   NVARCHAR (40) NOT NULL,
    [CreateDate] DATETIME      CONSTRAINT [DF_Roles_CreateDate] DEFAULT (getdate()) NOT NULL,
    [IsActive]   BIT           CONSTRAINT [DF_Roles_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Roles_name] UNIQUE NONCLUSTERED ([RoleName] ASC),
    CONSTRAINT [UQ_Role_RoleName] UNIQUE NONCLUSTERED ([RoleName] ASC)
);

