using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Data.Repositories;
using System;
using System.Web.Http;
using System.Net;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SystemChecker.Web.API
{
    [Route("api/[controller]")]
    public class Checks : Controller
    {
        private ICheckToPerformRepository _repository;

        public Checks(ICheckToPerformRepository repoFactory)
        {
            _repository = repoFactory;
        }

        // GET: api/Checks
        [HttpGet]
        public IEnumerable<CheckToPerform> Get()
        {
            try {
                var toDo = _repository.GetAll();
                return toDo.ToList();

                //return new List<CheckToPerform> { toDo.First() };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                throw ex;
            }
        }

        // GET api/Checks/5
        [HttpGet("{id}")]
        public CheckToPerform Get(int id)
        {
            var toDo = _repository.GetById(id);

            return toDo;
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
