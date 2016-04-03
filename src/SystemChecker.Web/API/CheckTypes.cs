using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using SystemChecker.Model.Data.Repositories;
using SystemChecker.Web.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SystemChecker.Web.API
{
    [Route("api/[controller]")]
    public class CheckTypes : Controller
    {
        private ICheckTypeRepository _repoFactory;

        public CheckTypes(ICheckTypeRepository repoFactory)
        {
            _repoFactory = repoFactory;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<CheckTypeDTO> Get()
        {
            return _repoFactory
                .GetAll()
                .ToList()
                .Select(x => new CheckTypeDTO
            {
                CheckTypeId = x.CheckTypeId,
                DisplayName = x.DisplayName
            });
        }
    }
}
