using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace SinExWebApp20328800.ViewModels
{
    public class FeeCalculateViewModel
    {
        public virtual FeeCalculateSearchViewModel param { get; set; }

        public virtual List<FeeCalculatePackageViewModel> packages { get; set; }

        [Required]
        public virtual string serviceType { get; set; }
        [Required]
        public virtual string currencyCode { get; set; }
    }
}