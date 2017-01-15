using Bookstore.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Bookstore.Controllers
{
    public class HomeController : Controller
    {
        private SearchEngine _searchEngine;

        public HomeController()
        {
            var amazonApiKey = WebConfigurationManager.AppSettings["AmazonApiKey"];
            var amazonSecret = WebConfigurationManager.AppSettings["AmazonSecret"];
            var amazonAssociateTag = WebConfigurationManager.AppSettings["AmazonAssociateTag"];
            _searchEngine = new SearchEngine(new List<IBookProvider>()
            {
                new Provider.Amazon(amazonApiKey, amazonSecret, amazonAssociateTag),
                new Provider.Apress()
            });
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = string.Empty;
            var empty = Enumerable.Empty<IBook>();
            return View(empty);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string title)
        {
            ViewBag.Title = title;
           
            var books = await _searchEngine.Search(title);
            return View(books);
        }
    }
}