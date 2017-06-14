using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;

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
}
