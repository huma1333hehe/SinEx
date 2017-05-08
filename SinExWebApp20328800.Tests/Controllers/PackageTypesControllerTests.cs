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
    public class PackageTypesControllerTests
    {
        [Test()]
        public void CreateTest()
        {
            PackageTypesController controller = new PackageTypesController();
            var result = controller.Create() as ViewResult;
            Assert.That(result, Is.Not.Null);
        }
    }
}