using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("PackageTypeSize")]
    public class PackageTypeSize
    {
        public virtual int PackageTypeSizeID { get; set; }
        [Display(Name = "Type Size")]
        public virtual string TypeSize { get; set; }
        [Display(Name = "Weight LImit")]
        public virtual string WeightLimit { get; set; }

        public virtual int PackageTypeID { get; set; }
        //
        public virtual PackageType PackageType { get; set; }
    }
}