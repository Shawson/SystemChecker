﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Repositories;
using Quartz;
using System;

namespace SystemChecker.Web.API
{
    [Route("api/[controller]")]
    public class Triggers : Controller
    {
        private ICheckTriggerRepository _repository;

        public Triggers(ICheckTriggerRepository repoFactory)
        {
            _repository = repoFactory;
        }

        // GET api/Triggers/5
        [HttpGet("{checkId}")]
        public IEnumerable<CheckTrigger> Get(int checkId)
        {
            return _repository.GetWhere(new { CheckId = checkId }).ToList();
        }

        // GET api/Triggers/5/1
        [HttpGet("{checkId}/{triggerId}")]
        public CheckTrigger Get(int checkId, int triggerId)
        {
            return _repository.GetFirst(new { CheckId = checkId, TriggerId = triggerId });
        }

        // GET api/Triggers/5
        [HttpPost("GetNextDate")]
        public DateTime? GetNextDate([FromBody]string expression)
        {
            var cronExpression = new CronExpression(expression);
            
            var offset = cronExpression.GetNextValidTimeAfter(new DateTimeOffset(DateTime.Now));

            if (offset.HasValue)
            {
                return offset.Value.DateTime;
            }
            
            return null;
        }

        // POST api/values
        [HttpPost("{checkId}")]
        public void Post(int checkId, [FromBody]CheckTrigger value)
        {
        }

        // PUT api/values/5
        [HttpPut("{checkId}/{triggerId}")]
        public void Put(int checkId, int triggerId, [FromBody]CheckTrigger value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{triggerId}")]
        public void Delete(int triggerId)
        {
        }
    }
}