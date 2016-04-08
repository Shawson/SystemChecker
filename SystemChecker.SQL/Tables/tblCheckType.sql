CREATE TABLE [dbo].[tblCheckType]
(
	[CheckTypeId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CheckAssembly] NCHAR(100) NULL, 
    [CheckTypeName] VARCHAR(200) NULL, 
    [DisplayName] VARCHAR(50) NULL
)
