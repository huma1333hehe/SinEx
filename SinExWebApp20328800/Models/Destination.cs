using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    [Table("Destination")]
    public class Destination
    {
        public virtual int DestinationID { get; set; }

        public string City { get; set; }
        [Display(Name = "Province")]
        public string ProvinceCode { get; set; }
        [Display(Name = "Currency")]
        public virtual string CurrencyCode { get; set; }
        //
        public virtual Currency Currency { get; set; }
    }
}