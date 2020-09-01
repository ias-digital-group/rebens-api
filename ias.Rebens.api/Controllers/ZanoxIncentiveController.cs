using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]"), Authorize("Bearer", Roles = "master,administratorRebens,publisherRebens")]
    [ApiController]
    public class ZanoxIncentiveController : BaseApiController
    {
        private IZanoxIncentiveRepository repo;

        public ZanoxIncentiveController(IZanoxIncentiveRepository zanoxIncentive) 
        {
            repo = zanoxIncentive;
        }

        

    }
}
