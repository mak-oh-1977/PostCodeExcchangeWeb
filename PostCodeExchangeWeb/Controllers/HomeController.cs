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

namespace PostCodeExchangeWeb.Controllers
{

    
    public class HomeController : Controller
    {
        private PostCode db = new PostCode();

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

       
        public class PostCodeMapper : CsvClassMap<list>
        {
            public PostCodeMapper()
            {
                Map(x => x.prefcd).Index(1);
                Map(x => x.pref).Index(7);
                Map(x => x.citycd).Index(2);
                Map(x => x.city).Index(9);
                Map(x => x.towncd).Index(3);
                Map(x => x.town).Index(11);
                Map(x => x.touri).Index(14);
                Map(x => x.choume).Index(15);
                Map(x => x.postcode).Index(4);

            }
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
            using (var r = new StreamReader(Server.MapPath("~/uploads/import.csv"), Encoding.GetEncoding("SHIFT_JIS")))
            using (var csv = new CsvReader(r))
            {
                // ヘッダーはないCSV
                csv.Configuration.HasHeaderRecord = true;
                // 先ほど作ったマッピングルールを登録
                csv.Configuration.RegisterClassMap<PostCodeMapper>();
                // データを読み出し
                var records = csv.GetRecords<list>();

                using (var context = new PostCode())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;

                    // 出力
                    foreach (var record in records)
                    {
                        // Addした段階ではSql文はDBに発行されない
                        context.list.Add(record);
 
                        Debug.WriteLine("{0}/{1}/{2}", record.postcode, record.pref, record.city);
                    }


                    // SaveChangesが呼び出された段階で初めてInsert文が発行される
                    context.SaveChanges();
                }
 
            }

            return View();
        }
    }
}