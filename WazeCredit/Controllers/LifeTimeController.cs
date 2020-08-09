using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WazeCredit.Service.LifeTimeExample;

namespace WazeCredit.Controllers
{
    public class LifeTimeController : Controller
    {
        private readonly TransientService _transientService;
        private readonly ScopedService _scopedService;
        private readonly SingletionService _singletionService;

        public LifeTimeController(TransientService transientService,
            ScopedService scopedService, SingletionService singletionService)
        {
            _transientService = transientService;
            _scopedService = scopedService;
            _singletionService = singletionService;
        }

        public IActionResult Index()
        {
            var messages = new List<string>
            {
                HttpContext.Items["CustomMiddlewareTransient"].ToString(),
                $"Transient Controller - {_transientService.GetGuid()}",
                HttpContext.Items["CustomMiddlewareScoped"].ToString(),
                $"Scoped Controller - {_scopedService.GetGuid()}",
                HttpContext.Items["CustomMiddlewareSingletion"].ToString(),
                $"Singletion Controller - {_singletionService.GetGuid()}",
            };

            return View(messages);
        }
    }
}
