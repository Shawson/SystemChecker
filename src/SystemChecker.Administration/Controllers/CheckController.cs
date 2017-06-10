using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SystemChecker.Administration.Controllers
{
    [Route("api/[controller]")]
    public class CheckController : Controller
    {
        private readonly IRepositoryFactory _repoFactory;

        public CheckController(IRepositoryFactory repoFactory)
        {
            _repoFactory = repoFactory;
        }

        // GET: api/values
        [HttpGet("[action]")]
        public IEnumerable<CheckToPerform> GetAll()
        {
            var repo = _repoFactory.GetCheckToPerformRepository();
            var everything = repo.GetAll().ToList();
            return everything.ToArray();
        }

        // GET api/values/5
        [HttpGet("[action]/{id}")]
        public CheckToPerform GetById(int id)
        {
            var repo = _repoFactory.GetCheckToPerformRepository();
            var item = repo.GetById(id);
            return item;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
