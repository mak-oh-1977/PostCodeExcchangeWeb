using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostCodeExchangeWeb;
using PostCodeExchangeWeb.Controllers;

namespace PostCodeExchangeWeb.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            var q = new HomeController.Query();
            // Act
            ViewResult result = controller.Index(q) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

    }
}
