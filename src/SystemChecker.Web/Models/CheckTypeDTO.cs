using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SystemChecker.Web.Models
{
    public class CheckTypeDTO
    {
        public int CheckTypeId { get; set; }
        public string DisplayName { get; set; }

    }
}
