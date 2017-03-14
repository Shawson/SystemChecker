using NSubstitute;
using System.Collections.Generic;
using SystemChecker.Model.Checkers;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Enums;
using SystemChecker.Model.Interfaces;
using SystemChecker.Model.QueryBuilder;
using Xunit;
using Microsoft.Extensions.Logging;

namespace SystemChecker.Model.Tests
{
    public class TestChecker : BaseChecker<string>, ISystemCheck
    {
        public CheckResult PerformCheck(ICheckResultRepository resultsRepo, ILogger logger)
        {
            var result = PassStatus(null, resultsRepo);

            return new CheckResult
            {
                Result = (int)result.SuccessStatus,
                FailureDetail = result.Description
            };
        }
    }

    public class BaseCheckerTests
    {
        [Fact]
        public void OneOutcome()
        {
            var checker = new TestChecker
            {
                Outcomes = new List<Outcome>
                {
                    new Outcome
                    {
                        SuccessStatus = SuccessStatus.Success,
                        Condition = new Condition
                        {
                            ValueA = true,
                            Comparator = QueryBuilder.Enums.Comparator.Equals,
                            ValueB = true
                        }
                    }
                }
            };

            Assert.Equal(checker.PerformCheck(Substitute.For<ICheckResultRepository>(), Substitute.For<ILogger>()).Result, (int)SuccessStatus.Success);
        }

        [Fact]
        public void MutlipleOutcomes()
        {
            var checker = new TestChecker
            {
                Outcomes = new List<Outcome>
                {
                    new Outcome
                    {
                        SuccessStatus = SuccessStatus.Success,
                        Condition = new Condition
                        {
                            Operator = QueryBuilder.Enums.ConditionType.AND,
                            Rules = new List<Condition>
                            {
                                new Condition
                                {
                                    ValueA = true,
                                    Comparator = QueryBuilder.Enums.Comparator.Equals,
                                    ValueB = true
                                },
                                new Condition
                                {
                                    ValueA = true,
                                    Comparator = QueryBuilder.Enums.Comparator.Equals,
                                    ValueB = false
                                }
                            }
                        }
                    },
                    new Outcome
                    {
                        SuccessStatus = SuccessStatus.SuccessWithWarnings,
                        Condition = new Condition
                        {
                            Operator = QueryBuilder.Enums.ConditionType.AND,
                            Rules = new List<Condition>
                            {
                                new Condition
                                {
                                    ValueA = true,
                                    Comparator = QueryBuilder.Enums.Comparator.Equals,
                                    ValueB = true
                                },
                                new Condition
                                {
                                    ValueA = true,
                                    Comparator = QueryBuilder.Enums.Comparator.Equals,
                                    ValueB = true
                                }
                            }
                        }
                    }
                }
            };

            Assert.Equal(checker.PerformCheck(Substitute.For<ICheckResultRepository>(), Substitute.For<ILogger>()).Result, (int)SuccessStatus.SuccessWithWarnings);
        }
    }
}
