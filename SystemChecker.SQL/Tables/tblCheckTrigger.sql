CREATE TABLE [dbo].[tblCheckTrigger]
(
	[TriggerId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CheckId] INT NOT NULL, 
    [CronExpression] VARCHAR(50) NOT NULL, 
    [PerformCatchUp] BIT NOT NULL DEFAULT 0,
	[Disabled] DATETIME NULL,
	[Updated] DATETIME NOT NULL,
)
