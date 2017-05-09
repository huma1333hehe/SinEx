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
    public class ManageControllerTests
    {
        ManageController controller;
        [OneTimeSetUp]
        public void TestSetUp()
        {
            controller = new ManageController();
        }

        [Test()]
        public void AddPhoneNumberTest()
        {
            ViewResult result = controller.AddPhoneNumber() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test()]
        public void ChangePasswordTest()
        {
            ViewResult result = controller.ChangePassword() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test()]
        public void SetPasswordTest()
        {
            ViewResult result = controller.SetPassword() as ViewResult;

            Assert.IsNotNull(result);
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            controller = null;
        }
    }
}