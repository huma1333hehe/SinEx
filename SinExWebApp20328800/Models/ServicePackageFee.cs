using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SinExWebApp20328800.Models
{
    [Table("ServicePackageFee")]
    public class ServicePackageFee
    {

        public virtual int ServicePackageFeeID { get; set; }
        [Display(Name = "Fee")]
        public virtual decimal Fee { get; set; }
        [Display(Name = "Minimum Fee")]
    
        public virtual decimal MinimumFee { get; set; }
        [Display(Name = "Package Type")]

        public virtual int PackageTypeID { get; set; }
        [Display(Name = "Service Type")]

        public virtual int ServiceTypeID { get; set; }
        //
        public virtual PackageType PackageType { get; set; }
        public virtual ServiceType ServiceType { get; set; }

    }
}