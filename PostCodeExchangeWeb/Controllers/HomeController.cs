using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Text;
using PostCodeExchangeWeb.Models;

namespace PostCodeExchangeWeb.Controllers
{


    public class HomeController : Controller
    {
        private PostCode db = new PostCode();

        private PostCodeData model = new PostCodeData();

        public class Query
        {
            public string address { get; set; }
        }



        public ActionResult Index(Query q)
        {
            Debug.WriteLine(q.address);

            if (!string.IsNullOrEmpty(q.address))
            {
                using (var context = new PostCode())
                {
                    int maxcd = context.pref.Max(x => x.prefcd);

                    // Addした段階ではSql文はDBに発行されない
                    context.pref.Add(new pref
                    {
                        prefcd = maxcd + 1,
                        name = q.address
                    });

                    // SaveChangesが呼び出された段階で初めてInsert文が発行される
                    context.SaveChanges();
                }

            }

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
                uploadFile.SaveAs(Server.MapPath("~/uploads/import.csv"));
            }

            return View();
        }

        [HttpPost]
        public ActionResult Import()
        {
            model.Import(Server.MapPath("~/uploads/import.csv"));

            return View();
        }


        [HttpPost]
        public ActionResult Find(string address)
        {
            ViewBag.res = model.Find(address);

            return View();
        }

    }
}