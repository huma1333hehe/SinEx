using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using SinExWebApp20328800.Models;

namespace SinExWebApp20328800.ViewModels
{
    public class FeeCalculatePackageViewModel
    {
        public virtual ServicePackageFee result { get; set; }
        [Required]
        public virtual string packageType { get; set; }
        [Required]
        public virtual string size { get; set; }
        [Required]
        public virtual decimal weight { get; set; }
        public virtual bool penalty { get; set; }
        public virtual decimal fee { get; set; }
        public virtual string limit { get; set; }
    }
}