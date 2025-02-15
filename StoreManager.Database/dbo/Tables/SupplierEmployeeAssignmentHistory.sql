CREATE TABLE [dbo].[SupplierEmployeeAssignmentHistory] (
    [Id]         INT      NOT NULL,
    [SupplierId] INT      NOT NULL,
    [EmployeeId] INT      NOT NULL,
    [StartDate]  DATETIME NOT NULL,
    [EndDate]    DATETIME NULL,
    CONSTRAINT [PK_EmployeeSupplierResponsibilityHistory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeSupplierResponsibilityHistory_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_EmployeeSupplierResponsibilityHistory_Suppliers] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[Suppliers] ([Id])
);

