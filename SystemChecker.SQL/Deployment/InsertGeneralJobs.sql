
insert into tblChecksToPerform (CheckTypeId, SystemName,Settings, Outcomes)
SELECT 
3,	'Ping Google', '{"HostNameOrAddress":"www.google.com","FailureResponseTimeoutMS": 3000}', '[
{"SuccessStatus":10,"Condition":{"Rules":null,"Operator":0,"ValueA":"{ThisRun.Status}","ValueB":0,"Comparator":0},"Description":null},
{"SuccessStatus":-20,"Condition":{"Rules":null,"Operator":0,"ValueA":"{ThisRun.Status}","ValueB":11010,"Comparator":0},"Description":"Timeout of {ThisRun.TimeOutMS}ms exceeded"},
{"SuccessStatus":-10,"Condition":{"Rules":null,"Operator":0,"ValueA":true,"ValueB":true,"Comparator":0},"Description":"Ping reply was {ThisRun.Status}"}]'
UNION SELECT 
2,	'Load Google Home Page', '{"Url":"http://www.google.com","FailureResponseTimeoutMS": 3000,"WarningResponseTimeoutSeconds":1000,"TextToFindInResponse":"Feeling Lucky","ExpectedResponseCode":200}'
GO

insert into tblCheckTrigger (CheckId, CronExpression)
		SELECT TOP 1 CheckId , '0 0/1 * 1/1 * ? *' from tblChecksToPerform where SystemName = 'Ping Google'
UNION	SELECT TOP 1 CheckId , '0 0/1 * 1/1 * ? *' from tblChecksToPerform where SystemName = 'Load Google Home Page'
GO