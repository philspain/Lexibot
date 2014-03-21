using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Threading;
using System.Data.Entity;

using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using LexibotData.Models;
using LexibotData.DataAccess;
using LexibotLogic.DIContainer;

namespace Lexibot.Controllers
{
    public class RequestsController : Controller
    {
        //
        // GET: /Requests/

        private static IEntityDbManager _dbManager;

        public RequestsController() :
            this(DependencyFactory.Resolve<IEntityDbManager>())
        { }

        public RequestsController(IEntityDbManager dbManager)
        {
            if (_dbManager != null)
            {
                _dbManager = dbManager;
            }
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [OutputCache(Duration = 120)]
        public string GetWordOccurrences()
        {
            List<WordOccurrenceDTO> occurrences = _dbManager.GetWordOccurrences();
            
            return JsonConvert.SerializeObject(occurrences).ToString();
        }
    }
}
