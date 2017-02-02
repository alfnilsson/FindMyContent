using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Toders.ContentFinder.Controllers
{
    [EPiServer.PlugIn.GuiPlugIn(
        Area = EPiServer.PlugIn.PlugInArea.AdminConfigMenu,
        Url = "/modules/Toders.ContentFinder/ContentFinder/Index",
        DisplayName = "My MVC Admin Plugin")]
    public class ContentFinderController : Controller
    {
        // GET: ContentFinder
        public ActionResult Index()
        {
            return View();
        }
    }
}