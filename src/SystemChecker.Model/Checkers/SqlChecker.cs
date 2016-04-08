using Dapper;
using System;
using System.Data.SqlClient;
using SystemChecker.Model.Data;
using SystemChecker.Model.Enums;
using SystemChecker.Model.Interfaces;
using System.Collections.Generic;
using SystemChecker.Model.Data.Repositories;
using System.Net.Sockets;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SystemChecker.Model.Checkers
{
    public class SqlChecker : BaseChecker<SqlCheckerSettings>, ISystemCheck
    {
        protected static IEnumerable<dynamic> RetrieveFromSQL(string connectionString, string sql, object addParameters)
        {
            IEnumerable<dynamic> resultList;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                try
                {
                    resultList = conn.Query<dynamic>(sql,
                        addParameters);
                }
                finally
                {
                    conn.Close();
                }
            }
            return resultList;
        }

        public virtual CheckResult PerformCheck(ICheckResultRepository resultsRepo)
        {
            // get the last check results for comparison
            var lastRun = GetLastRun(resultsRepo);


            try
            {
                var sqlResponse = RetrieveFromSQL(
                                    Settings.ConnectionString,
                                    Settings.Query, new { }).FirstOrDefault();

                var result = PassStatus(sqlResponse, resultsRepo);

                sqlResponse = null;

                return new CheckResult
                {
                    Result = (int)result.SuccessStatus,
                    FailureDetail = result.Description,
                    RunData = result.JsonRunData
                };

            }
            catch (SqlException seqlex)
            {
                var result = new CheckResult
                {
                    CheckDTS = DateTime.Now
                };

                if (seqlex.Number == -2)
                {
                    result.FailureDetail = "SQL Query Timeout";
                    result.Result = (int)SuccessStatus.ServerTimeout;
                    return result;
                }
                else
                {
                    result.FailureDetail = $"SQL Query Error {seqlex.ToString()}";
                    result.Result = (int)SuccessStatus.UnexpectedErrorDuringCheck;
                    return result;
                }
            }
            catch (SocketException sktex)
            {
                var result = new CheckResult
                {
                    CheckDTS = DateTime.Now
                };
                result.CheckDTS = DateTime.Now;
                result.FailureDetail = sktex.Message;
                result.Result = (int)SuccessStatus.ServerUnreachable;
                return result;
            }
            catch (Exception ex)
            {
                var result = new CheckResult
                {
                    CheckDTS = DateTime.Now
                };
                result.CheckDTS = DateTime.Now;
                result.FailureDetail = ex.ToString();
                result.Result = (int)SuccessStatus.UnexpectedErrorDuringCheck;
                return result;
            }
            finally
            {
                lastRun = null;
            }
        }
    }
    public class SqlCheckerSettings
    {
        public string ConnectionString { get; set; }
        public string Query { get; set; }
        public string ResultFieldName { get; set; }
        public string DurationFieldName { get; set; }
        public string LoggedRunIdFieldName { get; set; }
        public bool ExpectLoggedRunIdToAdvance { get; set; }
        public string RecordsAffectedFieldName { get; set; }
        public string LoggedRunDTSFieldName { get; set; }
    }
}
