using System.Collections.Generic;
using SystemChecker.Model.QueryBuilder;
using SystemChecker.Model.QueryBuilder.Enums;
using Xunit;
using Newtonsoft.Json.Linq;

namespace SystemChecker.Model.Tests
{
    public class QueryBuilderTests
    {
        public class WithObject
        {
            [Fact]
            public void And_Pass()
            {
                var dataForComparison = JObject.FromObject(new
                {
                    LastResult = new
                    {
                        RunId = 1,
                        Errors = 0,
                        ResultText = "Ok"
                    },
                    ThisResult = new
                    {
                        RunId = 2,
                        Errors = 0,
                        ResultText = "Fail"
                    }
                });

                var group = new Condition
                {
                    Operator = ConditionType.AND,
                    Rules = new List<Condition> {
                        new Condition
                        {
                            ValueA = "{ThisResult.RunId}",
                            Comparator = Comparator.GreaterThan,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = "{ThisResult.Errors}",
                            Comparator = Comparator.Equals,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = "{LastResult.RunId}",
                            Comparator = Comparator.LessThan,
                            ValueB = "{ThisResult.RunId}",
                        },
                        new Condition
                        {
                            ValueA = "{ThisResult.ResultText}",
                            Comparator = Comparator.Contains,
                            ValueB = "ai"
                        }
                    }
                };

                Assert.True(group.IsSatisfied(dataForComparison));
            }

            [Fact]
            public void And_Fail()
            {
                var dataForComparison = JObject.FromObject(new
                {
                    LastResult = new
                    {
                        RunId = 1,
                        Errors = 0,
                        ResultText = "Ok"
                    },
                    ThisResult = new
                    {
                        RunId = 1,
                        Errors = 0,
                        ResultText = "Fail"
                    }
                });

                var group = new Condition
                {
                    Operator = ConditionType.AND,
                    Rules = new List<Condition> {
                        new Condition
                        {
                            ValueA = "{ThisResult.RunId}",
                            Comparator = Comparator.GreaterThan,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = "{ThisResult.Errors}",
                            Comparator = Comparator.Equals,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = "{LastResult.RunId}",
                            Comparator = Comparator.LessThan,
                            ValueB = "{ThisResult.RunId}",
                        }
                    }
                };

                Assert.False(group.IsSatisfied(dataForComparison));
            }

        }

        public class WithoutObject
        {
            [Fact]
            public void And_Pass()
            {
                var group = new Condition
                {
                    Operator = ConditionType.AND,
                    Rules = new List<Condition> {
                        new Condition
                        {
                            ValueA = 1,
                            Comparator = Comparator.GreaterThan,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = 10,
                            Comparator = Comparator.Equals,
                            ValueB = 10
                        },
                        new Condition
                        {
                            ValueA = 100,
                            Comparator = Comparator.LessThan,
                            ValueB = 5,
                        }
                    }
                };

                Assert.False(group.IsSatisfied(null));
            }

            [Fact]
            public void And_Fail()
            {
                var group = new Condition
                {
                    Operator = ConditionType.AND,
                    Rules = new List<Condition> {
                        new Condition
                        {
                            ValueA = 1,
                            Comparator = Comparator.GreaterThan,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = 10,
                            Comparator = Comparator.Equals,
                            ValueB = 10
                        },
                        new Condition
                        {
                            ValueA = 3,
                            Comparator = Comparator.LessThan,
                            ValueB = 5,
                        }
                    }
                };

                Assert.True(group.IsSatisfied(null));
            }

            [Fact]
            public void Or_Pass()
            {
                var group = new Condition
                {
                    Operator = ConditionType.OR,
                    Rules = new List<Condition> {
                        new Condition
                        {
                            ValueA = 0,
                            Comparator = Comparator.GreaterThan,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = 10,
                            Comparator = Comparator.Equals,
                            ValueB = 10
                        },
                    }
                };

                Assert.True(group.IsSatisfied(null));
            }

            [Fact]
            public void Or_Fail()
            {
                var group = new Condition
                {
                    Operator = ConditionType.OR,
                    Rules = new List<Condition> {
                        new Condition
                        {
                            ValueA = 0,
                            Comparator = Comparator.GreaterThan,
                            ValueB = 0
                        },
                        new Condition
                        {
                            ValueA = 10,
                            Comparator = Comparator.Equals,
                            ValueB = 9
                        },
                    }
                };

                Assert.False(group.IsSatisfied(null));
            }
        }
     
        public class ComparatorTests
        {
            public class Equal
            {
                [Fact]
                public void Int_Pass()
                {
                    var group = new Condition
                    {
                        ValueA = 1,
                        Comparator = Comparator.Equals,
                        ValueB = 1
                    };

                    Assert.True(group.IsSatisfied(null));
                }

                [Fact]
                public void Int_Fail()
                {
                    var group = new Condition
                    {
                        ValueA = 1,
                        Comparator = Comparator.Equals,
                        ValueB = 2
                    };

                    Assert.False(group.IsSatisfied(null));
                }

                [Fact]
                public void String_Pass()
                {
                    var group = new Condition
                    {
                        ValueA = "echo",
                        Comparator = Comparator.Equals,
                        ValueB = "echo"
                    };

                    Assert.True(group.IsSatisfied(null));
                }

                [Fact]
                public void String_Fail()
                {
                    var group = new Condition
                    {
                        ValueA = "i pity the foo",
                        Comparator = Comparator.Equals,
                        ValueB = "who thinks this is equal"
                    };

                    Assert.False(group.IsSatisfied(null));
                }
            }

            public class Contains
            {
                [Fact]
                public void String_Pass()
                {
                    var group = new Condition
                    {
                        ValueA = "oh yea?",
                        Comparator = Comparator.Contains,
                        ValueB = "yea"
                    };

                    Assert.True(group.IsSatisfied(null));
                }

                [Fact]
                public void String_Fail()
                {
                    var group = new Condition
                    {
                        ValueA = "i pity the foo",
                        Comparator = Comparator.Contains,
                        ValueB = "apples"
                    };

                    Assert.False(group.IsSatisfied(null));
                }

                [Fact]
                public void Int_Pass()
                {
                    var group = new Condition
                    {
                        ValueA = 56,
                        Comparator = Comparator.Contains,
                        ValueB = 6
                    };

                    Assert.True(group.IsSatisfied(null));
                }

                [Fact]
                public void Int_Fail()
                {
                    var group = new Condition
                    {
                        ValueA = 56,
                        Comparator = Comparator.Contains,
                        ValueB = 0
                    };

                    Assert.False(group.IsSatisfied(null));
                }
            }
            
        }
    }
}
