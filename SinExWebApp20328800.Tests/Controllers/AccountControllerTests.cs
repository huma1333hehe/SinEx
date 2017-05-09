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
    public class AccountControllerTests
    {
        AccountController controller;
        [OneTimeSetUp]
        public void TestSetUp()
        {
            controller = new AccountController();
        }

        [Test()]
        public void LoginTest()
        {
            var result = controller.Login("Account/Login") as ViewResult;

            Assert.That(result, Is.Not.Null);
        }


        [Test()]
        public void RegisterTest()
        {
            var result = controller.Register("PersonalShippingAccount") as ViewResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test()]
        public void ForgotPasswordTest()
        {
            ViewResult result = controller.ForgotPassword() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestCase("testing##")]
        [TestCase("testing")]
        [TestCase("")]
        public void ResetPasswordTest(string code)
        {
            Assert.That(controller.ResetPassword(code), Is.Not.Null);
        }

        [Test()]
        public void ResetPasswordConfirmationTest()
        {
            ViewResult result = controller.ResetPasswordConfirmation() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test()]
        public void ExternalLoginFailureTest()
        {
            ViewResult result = controller.ExternalLoginFailure() as ViewResult;

            Assert.IsNotNull(result);
        }

        [OneTimeTearDown]
        public void TestTearDown()
        {
            controller = null;
        }
    }
}