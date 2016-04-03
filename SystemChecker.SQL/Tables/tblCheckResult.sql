CREATE TABLE [dbo].[tblCheckResult]
(
	[CheckResultId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CheckId] INT NOT NULL, 
    [CheckDTS] DATETIME NOT NULL, 
    [Result] INT NOT NULL, 
    [LoggedRunId] INT NULL, 
    [DurationMS] INT NULL, 
    [LoggedRunDTS] DATETIME NULL, 
    [RecordsAffected] INT NULL, 
    [FailureDetail] NVARCHAR(MAX) NULL, 
    [RunData] NVARCHAR(MAX) NULL 
)
