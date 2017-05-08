using NUnit.Framework;
using SinExWebApp20328800.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SinExWebApp20328800.Controllers.Tests
{
    [TestFixture()]
    public class CurrenciesControllerTests
    {
        [Test()]
        public void CreateTest()
        {
            CurrenciesController controller = new CurrenciesController();
            var result = controller.Create() as ViewResult;
            Assert.That(result, Is.Not.Null);
        }
    }
}