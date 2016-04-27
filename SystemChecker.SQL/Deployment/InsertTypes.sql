SET IDENTITY_INSERT tblCheckType ON
GO

insert into tblCheckType (CheckTypeId, CheckAssembly, CheckType, DisplayName)
	  SELECT 1, 'SystemChecker.Model',			'SystemChecker.Model.Checkers.SqlChecker',				'SQL Query Check'
UNION SELECT 2, 'SystemChecker.Model',			'SystemChecker.Model.Checkers.HttpChecker',				'HTTP Response Check'
UNION SELECT 3, 'SystemChecker.Model',			'SystemChecker.Model.Checkers.PingChecker',				'ICMP Ping Response Check'
UNION SELECT 4, 'SystemChecker.EmailCheckers',	'SystemChecker.EmailCheckers.SmtpHeloChecker',			'SMTP HELO Response Check'
UNION SELECT 5, 'SystemChecker.EmailCheckers',	'SystemChecker.EmailCheckers.EmailRoundtripChecker',	'Email Roundtrip Check'
GO

SET IDENTITY_INSERT tblCheckType OFF
GO