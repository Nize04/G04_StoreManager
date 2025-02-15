CREATE TABLE [dbo].[Accounts] (
    [Id]         INT            NOT NULL,
    [Email]      NVARCHAR (50)  NOT NULL,
    [Password]   NVARCHAR (150) NOT NULL,
    [Salt]       NVARCHAR (150) NOT NULL,
    [CreateDate] DATETIME       CONSTRAINT [DF_AccountDetails_CreateTime] DEFAULT (getdate()) NOT NULL,
    [UpdateDate] DATETIME       NULL,
    [IsActive]   BIT            CONSTRAINT [DF_EmployeeAccounts_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AccountDetails_Employees] FOREIGN KEY ([Id]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [email_unique] UNIQUE NONCLUSTERED ([Email] ASC)
);

