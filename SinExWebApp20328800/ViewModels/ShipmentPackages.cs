using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using SinExWebApp20328800.Models;

namespace SinExWebApp20328800.ViewModels
{
    public class ShipmentPackages
    {
        public virtual Shipment shipment { get; set; }
        public List<Package> packages { get; set; }
    }
}