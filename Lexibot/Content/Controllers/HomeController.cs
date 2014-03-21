using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Threading;
using System.Data.Entity;

namespace Lexibot.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /LexiBot/

        public ActionResult Index()
        {
            return View();
        }
    }
}
