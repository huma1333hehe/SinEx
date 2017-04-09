using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SinExWebApp20328800.Models;
using SinExWebApp20328800.ViewModels;
using System.Web.Mvc;

namespace SinExWebApp20328800.ViewModels
{
    public class ServicePackageFeesViewModel
    {
        public virtual IEnumerable<ServicePackageFee> Fees { get; set; }
        public virtual decimal Rate { get; set; }
        public virtual string CurrencyCode { get; set; }
        public virtual List<string> ServiceTypes { get; set; }
        public virtual List<string> PackageTypes { get; set; }
    }
}