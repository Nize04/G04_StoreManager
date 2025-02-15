CREATE TABLE [dbo].[AccountRoles] (
    [RoleId]    INT NOT NULL,
    [AccountId] INT NOT NULL,
    CONSTRAINT [PK_RoleAndPermissions_1] PRIMARY KEY CLUSTERED ([RoleId] ASC, [AccountId] ASC),
    CONSTRAINT [FK_EmployeeRoles_AccountDetails] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([Id]),
    CONSTRAINT [FK_RoleAndPermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);

