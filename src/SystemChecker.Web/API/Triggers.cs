using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using SystemChecker.Model.Data;
using Quartz;
using System;
using SystemChecker.Model.Data.Interfaces;

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
            return _repository.GetEnabledTriggersForCheckId(checkId).ToList();
        }

        // GET api/Triggers/5/1
        [HttpGet("{checkId}/{triggerId}")]
        public CheckTrigger Get(int checkId, int triggerId)
        {
            var trigger = _repository.GetById(triggerId);

            if (trigger != null && trigger.CheckId == checkId)
                return trigger;

            return null;
        }

        // GET api/Triggers/GetNextDate
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
