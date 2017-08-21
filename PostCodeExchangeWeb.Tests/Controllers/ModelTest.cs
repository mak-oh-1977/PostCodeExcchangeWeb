using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostCodeExchangeWeb.Models;

namespace PostCodeExchangeWeb.Tests.Controllers
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void 検索１()
        {
            var data = new PostCodeData();

            var res = data.Find("山形県酒田市宮野浦2");
            Assert.AreEqual(1, res.Count);


        }
    }
}
