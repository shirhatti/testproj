using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using testproj.Contexts;

namespace testproj.Controllers
{
    [Route("/")]
    public class HomeController : Controller
                


    {
        private ITableContext _tableContext;

        public HomeController(ITableContext context)
        {
            _tableContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET /5
        [HttpGet("{id}")]
        public void Get(string id)
        {
            Response.Redirect("http://google.com/?q=" + id);
            Response.StatusCode = 307;
        }

        [Route("/error/index")]
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
