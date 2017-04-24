using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("PackageType")]
    public class PackageType
    {  
        
        public virtual int PackageTypeID { get; set; }
        [Display(Name = "Package Type")]
        public virtual string Type { get; set; }
        public virtual string Description { get; set; }
        //
        public virtual ICollection<ServicePackageFee> ServicePackageFees { get; set; }
        public virtual ICollection<PackageTypeSize> PackageTypeSizes { get; set; }
    }
}