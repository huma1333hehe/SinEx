using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SinExWebApp20328800.Models;

namespace SinExWebApp20328800.ViewModels
{
    public class FeeCalculateViewModel
    {
        public virtual FeeCalculateSearchViewModel param { get; set; }

        public virtual ServicePackageFee result { get; set; }

        public virtual string origin { get; set; }
        public virtual string destination { get; set; }
        public virtual string packageType { get; set; }
        public virtual string size { get; set; }
        public virtual decimal? weight { get; set; }
        public virtual string serviceType { get; set; }
        public virtual string currencyCode { get; set; }
    }
}