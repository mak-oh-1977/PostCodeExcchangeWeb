using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostCodeExchangeWeb.Models;
using System.Transactions;

namespace PostCodeExchangeWeb.Tests.Controllers
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void 検索１()
        {
            using (TransactionScope transactionScope = new TransactionScope()) //終了時にロールバック
            {

                // テストデータの追加、テストの実施などを記述する
                var data = new PostCodeData();

                var res = data.Find("仙台市泉区上谷刈");
                Assert.AreEqual(0, res.Count);
            }
        }
    }
}
