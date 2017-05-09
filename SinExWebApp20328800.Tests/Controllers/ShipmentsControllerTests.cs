using NUnit.Framework;
using SinExWebApp20328800.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinExWebApp20328800.Controllers.Tests
{
    [TestFixture()]
    public class ShipmentsControllerTests
    {
        [TestCase("a", "b")]
        [TestCase("", "b")]
        [TestCase("a", "")]
        [TestCase("", "")]
        public void GetPayersTest(string ShipmentPayer, string TaxPayer)
        {
            ShipmentsController controller = new ShipmentsController();
            Assert.That(controller.GetPayers(ShipmentPayer, TaxPayer), Is.Not.Null);
        }

        [TestCase("")]
        [TestCase("1")]
        [TestCase("100")]
        public void GetCurrentDateTimeTest(string PickupType)
        {
            ShipmentsController controller = new ShipmentsController();
            Assert.That(controller.GetCurrentDateTime(PickupType), Is.Not.Null);
        }
    }
}