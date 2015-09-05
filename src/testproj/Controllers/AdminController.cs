using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using testsite.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace testproj.Controllers
{
    [Route("/admin/index")]
    public class AdminController : Controller
    {
        // GET: /<controller>/
        [Authorize("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
