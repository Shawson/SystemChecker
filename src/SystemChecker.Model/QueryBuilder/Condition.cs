using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SystemChecker.Model.QueryBuilder.Enums;

namespace SystemChecker.Model.QueryBuilder
{
    /// <summary>
    /// Pass a bunch of grouped conditions and a dynamically typed object and this will evaluate said object against the conditions
    /// </summary>
    public class Condition
    {
        // http://www.regexr.com/
        private static readonly Regex _fieldNameRegex = new Regex(@"\{(.*?)\}"); // find text enclosed in {}

        /// <summary>
        /// Optional child Rules to be evaluated
        /// </summary>
        public List<Condition> Rules { get; set; }

        /// <summary>
        /// How are the child rules of this condition to be treated?  
        /// Ignored if no child rules exist
        /// </summary>
        public ConditionType Operator { get; set; }

        /// <summary>
        /// Can be a raw value for comparison or a string encased in {} which will be treated as a property name and will be evaluated.
        /// You can navigate to sub properties eg {MyObject.Property.SomeName}
        /// </summary>
        public object ValueA { get; set; }

        /// <summary>
        /// Can be a raw value for comparison or a string encased in {} which will be treated as a property name and will be evaluated.
        /// You can navigate to sub properties eg {MyObject.Property.SomeName}
        /// </summary>
        public object ValueB { get; set; }

        /// <summary>
        /// Which comparator to use for the operation
        /// </summary>
        public Comparator Comparator { get; set; }

        /// <summary>
        /// Should contain information about which condition caused the failure
        /// </summary>
        public string FailureDetail { get; set; }

        public bool IsSatisfied(JObject subject)
        {
            if (ValueA != null && ValueB != null)
            {
                var a = _fieldNameRegex.Match(ValueA.ToString()).Success 
                    ? GetPropertyValue(subject, _fieldNameRegex.Match(ValueA.ToString()).Groups[1].Value)
                    : ValueA;

                var b = _fieldNameRegex.Match(ValueB.ToString()).Success
                    ? GetPropertyValue(subject, _fieldNameRegex.Match(ValueB.ToString()).Groups[1].Value)
                    : ValueB;

                switch(Comparator)
                {
                    case Comparator.GreaterThan:
                        if (!(a > b))
                        {
                            FailureDetail = $"Condition Failed : {ValueA}({a ?? "null"}) {Comparator} {ValueB}({b ?? "null"})";
                            return false;
                        }
                        break;

                    case Comparator.LessThan:
                        if (!(a < b))
                        {
                            FailureDetail = $"Condition Failed : {ValueA}({a ?? "null"}) {Comparator} {ValueB}({b ?? "null"})";
                            return false;
                        }
                        break;

                    case Comparator.Equals:
                        if (!(a == b))
                        {
                            FailureDetail = $"Condition Failed : {ValueA}({a ?? "null"}) {Comparator} {ValueB}({b ?? "null"})";
                            return false;
                        }
                        break;

                    case Comparator.StartsWith:
                        if (!a.ToString().StartsWith(b.ToString()))
                        {
                            FailureDetail = $"Condition Failed : {ValueA}({a ?? "null"}) {Comparator} {ValueB}({b ?? "null"})";
                            return false;
                        }
                        break;

                    case Comparator.Contains:
                        if (!a.ToString().Contains(b.ToString()))
                        {
                            FailureDetail = $"Condition Failed : {ValueA}({a ?? "null"}) {Comparator} {ValueB}({b ?? "null"})";
                            return false;
                        }
                        break;

                    case Comparator.IsNull:
                        if (a != null)
                        {
                            FailureDetail = $"Condition Failed : {ValueA}({a ?? "null"}) {Comparator}";
                            return false;
                        }
                        break;
                }
            }

            if (Rules != null)
            {
                switch(Operator) {
                    case ConditionType.AND:
                        // all must pass
                        foreach (var rule in Rules)
                        {
                            if (!rule.IsSatisfied(subject))
                            {
                                FailureDetail = rule.FailureDetail;
                                return false;
                            }
                        }

                        break;
                    case ConditionType.OR:
                        // any can pass
                        var orFailureDetail = string.Empty;
                        foreach (var rule in Rules)
                        {
                            if (rule.IsSatisfied(subject))
                            {
                                return true;
                            }
                            else
                            {
                                orFailureDetail += rule.FailureDetail;
                            }
                        }

                        FailureDetail = orFailureDetail;
                        return false;
                }
            }

            return true;
        }

        protected static dynamic GetPropertyValue(JObject o, string property)
        {
            if (property.Contains("."))
            {
                var propertyComponents = property.Split('.');

                // check for nulls
                var token = o[propertyComponents[0]];

                if (token == null ||
                    token.Type == JTokenType.Array && !token.HasValues ||
                    token.Type == JTokenType.Object && !token.HasValues ||
                    token.Type == JTokenType.String && token.ToString() == String.Empty ||
                    token.Type == JTokenType.Null)
                {
                    return null;
                }

                return GetPropertyValue((JObject)o[propertyComponents[0]], propertyComponents[1]);
            }
            else {
                //return o.GetType()?.GetProperty(property)?.GetValue(o, null) ?? null;
                return o[property];
            }
        }
    }
}
