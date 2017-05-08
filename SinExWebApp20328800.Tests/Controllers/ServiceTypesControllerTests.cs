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
    public class ServiceTypesControllerTests
    {
        [Test()]
        public void CreateTest()
        {
            ServiceTypesController controller = new ServiceTypesController();
            var result = controller.Create() as ViewResult;
            Assert.That(result, Is.Not.Null);
        }
    }
}