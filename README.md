System Checker
==============

** This is an ongoing piece of developement and by no means finished yet!  Interfaces may still change as might SQL schemas **

This was created for me to start playing with the dot net next stuff, which in itself is still in a bit of flux- so beware!

This is a quick and fairly simple tool for monitoring the state of various systems we have running.  You can set the app up with a number of jobs, each with a number of scheduled triggers and the app will churn away firing these jobs and recording the result to SQL.  The docs here are aimed at developers- I'll flesh it out for actual use when the software is a bit more finished.

Each job is an instance of a "Checker"- you can write your own checkers and add them to the app by writing a new class which inherits from the BaseChecker<> base class and implements the ISystemCheck interface.  Once ready, add an entry to the tblCheckType table to make the system aware.  

## Creating a new checker

When creating a new Checker you inherit from BaseChecker<> which is a generic class, the generic type passed in being the type of the settings object used for your checker.  The settings class is a simple POCO with a bunch of properties- the whole thing gets serialised to JSON and stored in SQL- this is where users can set properties to be loaded in by your checker when the check is run- for example on the SQLChecker the settings class contains a property for "connection string" and "query to run".

Tests all produce a result anonymous object which is serliased and stored.  The outcome of a test is decided in the base class based on user configuration.  It's important that the checker itself should just be grabbing and storing data- no decisions on pass or failure should be made in the test itself.  The PassStatus method of the base class deserialised the json outcomes data thats associated with each entry in tblChecksToPerform and parses that set of rules to figure out the outcome.

The body of the PerformCheck method of each check returns a check result.  The pattern followed in all checks is;

```c#
public CheckResult PerformCheck(ICheckResultRepository resultsRepo)
{
	// Some setup here- reading stuff from Settings etc

	try {
		// do the actual test

		// build an anonymous object which will store the results for this check
		// this will be serialised and stored to sql so you can compare results between runs
		var thisRun = new
        {
            Status = (int)reply.Status,
            RoundtripTime = (int)reply.RoundtripTime
        };

        // call the result into the base class's PassStatus method
        var result = PassStatus(thisRun, resultsRepo);

        // pass it back
        return new CheckResult
        {
            Result = (int)result.SuccessStatus,
            FailureDetail = result.Description,
            DurationMS = (int)reply.RoundtripTime,
            RunData = result.JsonRunData
        };
	}
	// be sure to capture any problems with your check!
	catch (Exception ex)
    {
        return new CheckResult
        {
            FailureDetail = ex.Message,
            Result = (int)SuccessStatus.UnexpectedErrorDuringCheck
        };
    }
}
```



### Current checkers

* SystemChecker.Model.Checkers.SqlChecker - SQL Query Check

  This check will run the specified query against the specified connection string and store the results.  

* SystemChecker.Model.Checkers.HttpChecker - HTTP Response Check

  This will make an HTTP request and store the response times, content, response codes etc.

SystemChecker.Model.Checkers.PingChecker - ICMP Ping Response Check

  This will perform a standard ICMP 'Ping' at the given server.

SystemChecker.Model.Checkers.SmtpHeloChecker - SMTP HELO Response Check

  This will try to connect to an SMTP server and then issue a HELO command and store the response.


## Adding Jobs

This is done in sql- there are a couple of examples you can see in SQLChecker.SQL/Deployment/InsertGeneralJobs.sql

## Other libraries used

Scheulding uses the Quartz C# library.

The data layer uses Dapper as it's more light weight and thread safe, unlike entity framework.  I used a generic repository & sql generator written by Diego Garcia to save cluttering the code with sql.
https://github.com/Yoinbol/Dapper.DataRepositories
https://github.com/Yoinbol/MicroOrm.Pocos.SqlGenerator

