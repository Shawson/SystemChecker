﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Repositories;

namespace SystemChecker.Model.Checkers
{
    public abstract class BaseChecker<T> 
    {
        public int CheckToPerformId { get; set; }

        public T Settings { get; set; }

        public List<Outcome> Outcomes { get; set; } = null;

        public CheckResult GetLastRun(ICheckResultRepository repo)
        {
            return repo.GetLastRun(CheckToPerformId);
        }
        
        /// <summary>
        /// Use the outcomes collection to interrogate the current run results and establish an outcome
        /// </summary>
        /// <param name="thisRunData">A dynamic object which holds result data of the current run</param>
        /// <returns></returns>
        protected Outcome PassStatus(dynamic thisRun, ICheckResultRepository repo)
        {
            var thisRunData = JObject.FromObject(thisRun);

            var lastRun = GetLastRun(repo);
            var lastRunData = (lastRun == null)
                ? null
                : string.IsNullOrWhiteSpace(lastRun.RunData) 
                        ? null 
                        : JObject.Parse(lastRun.RunData); 

            var runResult = new JObject();
            runResult.Add("ThisRun", thisRunData);
            runResult.Add("LastRun", lastRunData);

            foreach (var outcome in Outcomes)
            {
                if (outcome.Condition.IsSatisfied(runResult))
                {
                    outcome.JsonRunData = JsonConvert.SerializeObject(thisRunData);
                    return outcome;
                }
            }

            return new Outcome
            {
                JsonRunData = JsonConvert.SerializeObject(thisRunData),
                SuccessStatus = Enums.SuccessStatus.Failure,
                Description = "Failure"
            };
        }
    }
}