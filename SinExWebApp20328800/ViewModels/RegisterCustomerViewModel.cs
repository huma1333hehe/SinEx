using SinExWebApp20328800.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SinExWebApp20328800.ViewModels
{
    public class RegisterCustomerViewModel
    {
        public PersonalShippingAccount PersonalInformation { get; set; }
        public BusinessShippingAccount BusinessInformation { get; set; }
        public RegisterViewModel LoginInformation { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }
    }
}