using PostCodeExchangeWeb.Models;
using System.Web;
using System.Web.Mvc;

namespace PostCodeExchangeWeb.Controllers
{
    public class HomeController : Controller
    {
        private PostCodeData model = new PostCodeData();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string address)
        {
            ViewBag.res = model.Find(address);

            return View();
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileWrapper uploadFile)
        {
            if (uploadFile != null)
            {
                uploadFile.SaveAs(Server.MapPath("~/import.csv"));

                model.Import(Server.MapPath("~/import.csv"));

                ViewBag.Result = "完了しました";
            }

            return View();
        }



    }
}