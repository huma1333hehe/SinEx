using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SinExWebApp20328800.Models
{
    public class SinExWebApp20328800DatabaseContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public SinExWebApp20328800DatabaseContext() : base("name=SinExWebApp20328800DatabaseContext")
        {
        }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.PackageType> PackageTypes { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.ServiceType> ServiceTypes { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.ServicePackageFee> ServicePackageFees { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Currency> Currencies { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Destination> Destinations { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.PackageTypeSize> PackageTypeSizes { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Shipment> Shipments { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.ShippingAccount> ShippingAccounts { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.PickupLocation> PickupLocations { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Recipient> Recipients { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Package> Packages { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Tracking> Trackings { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Payment> Payments { get; set; }

        public System.Data.Entity.DbSet<SinExWebApp20328800.Models.Penalty> Penalties { get; set; }

    }
}
