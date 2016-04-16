CREATE TABLE [dbo].[tblChecksToPerform]
(
	[CheckId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SystemName] VARCHAR(100) NULL, 
    [Settings] NVARCHAR(MAX) NULL, 
    [Outcomes] NVARCHAR(MAX) NULL,
	[CheckTypeId] INT NOT NULL DEFAULT 0, 
    [CheckSuiteId] INT NULL, 
    [Disabled] DATETIME NULL,
	[Updated] DATETIME NOT NULL DEFAULT getdate()
)
