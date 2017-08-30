using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PostCodeExchangeWeb.Models
{
    public class PostCodeData
    {
        private PostCode db = new PostCode();

        public class PostCodeMapper : CsvClassMap<list>
        {
            public PostCodeMapper()
            {
                Map(x => x.prefcd).Index(1);
                Map(x => x.jigyousho).Index(5);
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

        public void  Import(string path)
        {
            using (var r = new StreamReader(path, Encoding.GetEncoding("SHIFT_JIS")))
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

                    context.Database.ExecuteSqlCommand("delete from list");
                    context.Database.ExecuteSqlCommand("delete from city");
                    context.Database.ExecuteSqlCommand("delete from pref");

                    // 出力
                    foreach (var record in records)
                    {
                        // Addした段階ではSql文はDBに発行されない
                        if (record.jigyousho == 0)
                        {
                            context.list.Add(record);

                            Debug.WriteLine("{0}/{1}/{2}", record.postcode, record.pref, record.city);
                        }
                        else
                        {
                            Debug.WriteLine("{0}/{1}/{2}", record.postcode, record.pref, record.city);

                        }
                    }


                    // SaveChangesが呼び出された段階で初めてInsert文が発行される
                    context.SaveChanges();


                    context.Database.ExecuteSqlCommand("insert into city (prefcd, pref, citycd, city) select distinct prefcd, pref, citycd, city from list");
                    context.Database.ExecuteSqlCommand("insert into pref (prefcd, name) select distinct prefcd, pref from list");
                    context.Database.ExecuteSqlCommand("update city set ryaku = city where charindex(N'郡', city) = 0");
                    context.Database.ExecuteSqlCommand("update city set ryaku = city where city in (N'郡山市', N'蒲郡市', N'小郡市')");
                    context.Database.ExecuteSqlCommand("update city set ryaku = substring(city, charindex(N'郡', city) + 1, len(city)) where ryaku is null");

                }

            }


        }

        public class PostCodeRes
        {
            public string Code { set; get; }
            public string Name { set; get; }
        }


        public List<PostCodeRes> Find(string address)
        {
            int prefcd;

            var ret = FindPref(address, out prefcd);
            if (ret != "")
                address = ret;

            int citycd;
            ret = FindCity(address, out citycd, prefcd);
            if (citycd != -1)
                address = ret;

            if (citycd == -1)
                address = FindCityRyaku(address, out citycd, prefcd);

            var list = FindTown(address, citycd);

            var res = FindPostCode(address, list);

            return res;
        }

        protected string FindPref(string address, out int prefcd)
        {
            var r = db.pref.Where(x => x.name.StartsWith(address.Substring(0, 2))).
                Select(x => new { prefcd = x.prefcd, pref = x.name }).
                FirstOrDefault();

            if (r == null)
            {
                prefcd = -1;
                return "";
            }

            prefcd = r.prefcd;

            int len = r.pref.Trim().Length;
            if (address.Substring(0, len) == r.pref.Trim())
            {
                return address.Substring(len);
            }

            return address;

        }

        protected string FindCity(string address, out int citycd, int prefcd)
        {
            citycd = -1;

            int retcd = -1;
            string retad = "";
            if (prefcd != -1)
            {
                var ret = db.city.Where(x => x.prefcd == prefcd & x.city1.StartsWith(address.Substring(0, 2))).ToList();

                if (ret != null)
                {
                    ret.ForEach(x =>
                    {
                        int cd = x.citycd;
                        int len = x.city1.Length;
                        if (len > address.Length)
                            len = address.Length;
                        if (address.Substring(0, len) == x.city1)
                        {
                            retad = address.Substring(len);
                            retcd = cd;
                        }

                    }
                    );
                }
            }
            else
            {
                var ret = db.city.Where(x => x.city1.StartsWith(address.Substring(0, 2))).ToList();

                if (ret != null)
                {
                    ret.ForEach(x =>
                    {
                        int cd = x.citycd;
                        int len = x.city1.Length;
                        if (len > address.Length)
                            len = address.Length;
                        if (address.Substring(0, len) == x.city1)
                        {
                            retad = address.Substring(len);
                            retcd = cd;
                        }

                    }
                    );
                }
            }

            citycd = retcd;

            return retad;
        }

        protected string FindCityRyaku(string address, out int citycd, int prefcd)
        {
            citycd = -1;

            city ret;
            if (prefcd != -1)
            {
                ret = db.city.Where(x => x.prefcd == prefcd & x.ryaku.StartsWith(address.Substring(0, 2))).FirstOrDefault();

            }
            else
            {
                ret = db.city.Where(x => x.ryaku.StartsWith(address.Substring(0, 2))).FirstOrDefault();
            }

            if (ret != null)
            {
                citycd = ret.citycd;
                int len = ret.city1.Length;
                if (len > address.Length)
                    len = address.Length;
                if (address.Substring(0, len) == ret.city1)
                {
                    return address.Substring(len);
                }
            }
            return "";

        }

        protected List<int> FindTown(string address, int citycd)
        {

            var ret = db.list.Where(x => x.citycd == citycd)
                    .GroupBy(c => new { c.towncd, c.town })
                    .OrderByDescending(x => x.Key.town.Length)
                    .Select(x => new { towncd = x.Key.towncd, town = x.Key.town })
                    .ToList();

            var res = new List<int>();
            int minpos = address.Length;
            int maxlen = 0;
            ret.ForEach(x =>
            {
                if(address.Length != 0)
                {
                    int pos = address.IndexOf(x.town);
                    if (pos >= 0 && pos <= minpos)
                    {
                        int len = x.town.Length;
                        if (len > maxlen)
                        {
                            res.Add((int)x.towncd);
                            maxlen = len;
                            minpos = pos;
                        }
                    }
                }
                else
                {
                    res.Add((int)x.towncd);
                }
            });

            return res;
        }

        protected List<PostCodeRes> FindPostCode(string address, List<int> towncd)
        {

            var res = db.list.Where(x => towncd.Contains((int)x.towncd)).
                    Distinct().
                    Select(x =>
                    new {
                        Code = x.postcode,
                        Choume = x.choume,
                        Name = x.pref + x.city + x.touri + x.town + x.choume
                    }).ToList();

            var ret = new List<PostCodeRes>();
            res.ForEach(x =>
            {
                string s = x.Choume.Replace(" ", "");
                int pos = address.IndexOf(s);
                if (pos >= 0)
                {
                    ret.Add(new PostCodeRes() { Code = x.Code, Name = x.Name });
                }
            });

            return ret;
        }

    }
}