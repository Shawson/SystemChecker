using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.QueryBuilder.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SystemChecker.Administration.Controllers
{
    [Route("api/[controller]")]
    public class CheckSuiteController : Controller
    {
        private readonly IRepositoryFactory _repoFactory;

        public CheckSuiteController(IRepositoryFactory repoFactory)
        {
            _repoFactory = repoFactory;
        }

        // GET: api/values
        [HttpGet("[action]")]
        public IEnumerable<CheckSuite> GetAll()
        {
            var repo = _repoFactory.GetCheckSuiteRepository();
            var everything = repo
                .GetAll()
                .OrderBy(x => x.SuiteName)
                .ToList();
            return everything.ToArray();
        }
    }

    [Route("api/[controller]")]
    public class LookupController : Controller
    {
        public LookupController()
        {
        }

        // GET: api/values
        [HttpGet("[action]")]
        public Dictionary<int, string> Comparators()
        {
            Array values = Enum.GetValues(typeof(Comparator));
            var retVal = new Dictionary<int, string>();

            foreach (Comparator val in values)
            {
                retVal.Add((int)val, Enum.GetName(typeof(Comparator), val));
            }

            return retVal;
        }

        // GET: api/values
        [HttpGet("[action]")]
        public Dictionary<int, string> Operators()
        {
            Array values = Enum.GetValues(typeof(ConditionType));
            var retVal = new Dictionary<int, string>();

            foreach (ConditionType val in values)
            {
                retVal.Add((int)val, Enum.GetName(typeof(ConditionType), val));
            }

            return retVal;
        }
    }
}
