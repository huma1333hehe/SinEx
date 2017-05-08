namespace SinExWebApp20328800.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class SinExConfiguration : DbMigrationsConfiguration<SinExWebApp20328800.Models.SinExWebApp20328800DatabaseContext>
    {
        public SinExConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "SinExWebApp20328800.Models.SinExWebApp20328800DatabaseContext";
        }

        protected override void Seed(SinExWebApp20328800.Models.SinExWebApp20328800DatabaseContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            // Add package type data.

            //nnnn
            context.PackageTypes.AddOrUpdate(
                p => p.PackageTypeID,
                new PackageType { PackageTypeID = 1, Type = "Envelope", Description = "for correspondence and documents only with no commercial value" },
                new PackageType { PackageTypeID = 2, Type = "Pak", Description = "for flat, non-breakable articles including heavy documents" },
                new PackageType { PackageTypeID = 3, Type = "Tube", Description = "for larger documents, such as blueprints, which should be rolled rather than folded" },
                new PackageType { PackageTypeID = 4, Type = "Box", Description = "for bulky items, such as electronic parts and textile samples" },
                new PackageType { PackageTypeID = 5, Type = "Customer", Description = "for packaging provided by customer" }
                );

            // Add service type data.
            context.ServiceTypes.AddOrUpdate(
                p => p.ServiceTypeID,
                new ServiceType { ServiceTypeID = 1, Type = "Same Day", CutoffTime = "10:00 a.m.", DeliveryTime = "Same day" },
                new ServiceType { ServiceTypeID = 2, Type = "Next Day 10:30", CutoffTime = "3:00 p.m.", DeliveryTime = "Next day 10:30 a.m." },
                new ServiceType { ServiceTypeID = 3, Type = "Next Day 12:00", CutoffTime = "6:00 p.m.", DeliveryTime = "Next day 12:00 p.m." },
                new ServiceType { ServiceTypeID = 4, Type = "Next Day 15:00", CutoffTime = "9:00 p.m.", DeliveryTime = "Next day 15:00 p.m." },
                new ServiceType { ServiceTypeID = 5, Type = "2nd Day", CutoffTime = "", DeliveryTime = "5:00 p.m. second business day" },
                new ServiceType { ServiceTypeID = 6, Type = "Ground", CutoffTime = "", DeliveryTime = "1 to 5 business days" }
                );

            // Add service and package type fees.
            context.ServicePackageFees.AddOrUpdate(
                p => p.ServicePackageFeeID,
                // Same Day
                new ServicePackageFee { ServicePackageFeeID = 1, Fee = 160, MinimumFee = 160, ServiceTypeID = 1, PackageTypeID = 1 }, // Envelope
                new ServicePackageFee { ServicePackageFeeID = 2, Fee = 100, MinimumFee = 160, ServiceTypeID = 1, PackageTypeID = 2 }, // Pak
                new ServicePackageFee { ServicePackageFeeID = 3, Fee = 100, MinimumFee = 160, ServiceTypeID = 1, PackageTypeID = 3 }, // Tube
                new ServicePackageFee { ServicePackageFeeID = 4, Fee = 110, MinimumFee = 160, ServiceTypeID = 1, PackageTypeID = 4 }, // Box
                new ServicePackageFee { ServicePackageFeeID = 5, Fee = 110, MinimumFee = 160, ServiceTypeID = 1, PackageTypeID = 5 }, // Customer
                                                                                                                                      // Next Day 10:30
                new ServicePackageFee { ServicePackageFeeID = 6, Fee = 140, MinimumFee = 140, ServiceTypeID = 2, PackageTypeID = 1 }, // Envelope
                new ServicePackageFee { ServicePackageFeeID = 7, Fee = 90, MinimumFee = 140, ServiceTypeID = 2, PackageTypeID = 2 }, // Pak
                new ServicePackageFee { ServicePackageFeeID = 8, Fee = 90, MinimumFee = 140, ServiceTypeID = 2, PackageTypeID = 3 }, // Tube
                new ServicePackageFee { ServicePackageFeeID = 9, Fee = 99, MinimumFee = 100, ServiceTypeID = 2, PackageTypeID = 4 }, // Box
                new ServicePackageFee { ServicePackageFeeID = 10, Fee = 99, MinimumFee = 140, ServiceTypeID = 2, PackageTypeID = 5 }, // Customer
                                                                                                                                      // Next Day 12:00
                new ServicePackageFee { ServicePackageFeeID = 11, Fee = 130, MinimumFee = 130, ServiceTypeID = 3, PackageTypeID = 1 }, // Envelope
                new ServicePackageFee { ServicePackageFeeID = 12, Fee = 80, MinimumFee = 130, ServiceTypeID = 3, PackageTypeID = 2 }, // Pak
                new ServicePackageFee { ServicePackageFeeID = 13, Fee = 80, MinimumFee = 130, ServiceTypeID = 3, PackageTypeID = 3 }, // Tube
                new ServicePackageFee { ServicePackageFeeID = 14, Fee = 88, MinimumFee = 130, ServiceTypeID = 3, PackageTypeID = 4 }, // Box
                new ServicePackageFee { ServicePackageFeeID = 15, Fee = 88, MinimumFee = 130, ServiceTypeID = 3, PackageTypeID = 5 }, // Customer
                                                                                                                                      // Next Day 15:00
                new ServicePackageFee { ServicePackageFeeID = 16, Fee = 120, MinimumFee = 120, ServiceTypeID = 4, PackageTypeID = 1 }, // Envelope
                new ServicePackageFee { ServicePackageFeeID = 17, Fee = 70, MinimumFee = 120, ServiceTypeID = 4, PackageTypeID = 2 }, // Pak
                new ServicePackageFee { ServicePackageFeeID = 18, Fee = 70, MinimumFee = 120, ServiceTypeID = 4, PackageTypeID = 3 }, // Tube
                new ServicePackageFee { ServicePackageFeeID = 19, Fee = 77, MinimumFee = 120, ServiceTypeID = 4, PackageTypeID = 4 }, // Box
                new ServicePackageFee { ServicePackageFeeID = 20, Fee = 77, MinimumFee = 120, ServiceTypeID = 4, PackageTypeID = 5 }, // Customer
                                                                                                                                      // 2nd Day
                new ServicePackageFee { ServicePackageFeeID = 21, Fee = 50, MinimumFee = 50, ServiceTypeID = 5, PackageTypeID = 1 }, // Envelope
                new ServicePackageFee { ServicePackageFeeID = 22, Fee = 50, MinimumFee = 50, ServiceTypeID = 5, PackageTypeID = 2 }, // Pak
                new ServicePackageFee { ServicePackageFeeID = 23, Fee = 50, MinimumFee = 50, ServiceTypeID = 5, PackageTypeID = 3 }, // Tube
                new ServicePackageFee { ServicePackageFeeID = 24, Fee = 55, MinimumFee = 55, ServiceTypeID = 5, PackageTypeID = 4 }, // Box
                new ServicePackageFee { ServicePackageFeeID = 25, Fee = 55, MinimumFee = 55, ServiceTypeID = 5, PackageTypeID = 5 }, // Customer
                                                                                                                                     // Ground
                new ServicePackageFee { ServicePackageFeeID = 26, Fee = 25, MinimumFee = 25, ServiceTypeID = 6, PackageTypeID = 1 },// Envelope
                new ServicePackageFee { ServicePackageFeeID = 27, Fee = 25, MinimumFee = 25, ServiceTypeID = 6, PackageTypeID = 2 }, // Pak
                new ServicePackageFee { ServicePackageFeeID = 28, Fee = 25, MinimumFee = 25, ServiceTypeID = 6, PackageTypeID = 3 }, // Tube
                new ServicePackageFee { ServicePackageFeeID = 29, Fee = 30, MinimumFee = 30, ServiceTypeID = 6, PackageTypeID = 4 }, // Box
                new ServicePackageFee { ServicePackageFeeID = 30, Fee = 30, MinimumFee = 30, ServiceTypeID = 6, PackageTypeID = 5 }  // Customer
                );

            // Add destination data.
            context.Destinations.AddOrUpdate(
                p => p.DestinationID,
                new Destination { DestinationID = 1, City = "Beijing", ProvinceCode = "BJ", CurrencyCode = "CNY" },
                new Destination { DestinationID = 2, City = "Changchun", ProvinceCode = "JL", CurrencyCode = "CNY" },
                new Destination { DestinationID = 3, City = "Changsha", ProvinceCode = "HN", CurrencyCode = "CNY" },
                new Destination { DestinationID = 4, City = "Chengdu", ProvinceCode = "SC", CurrencyCode = "CNY" },
                new Destination { DestinationID = 5, City = "Chongqing", ProvinceCode = "CQ", CurrencyCode = "CNY" },
                new Destination { DestinationID = 6, City = "Fuzhou", ProvinceCode = "JX", CurrencyCode = "CNY" },
                new Destination { DestinationID = 7, City = "Golmud", ProvinceCode = "QH", CurrencyCode = "CNY" },
                new Destination { DestinationID = 8, City = "Guangzhou", ProvinceCode = "GD", CurrencyCode = "CNY" },
                new Destination { DestinationID = 9, City = "Guiyang", ProvinceCode = "GZ", CurrencyCode = "CNY" },
                new Destination { DestinationID = 10, City = "Haikou", ProvinceCode = "HI", CurrencyCode = "CNY" },
                new Destination { DestinationID = 11, City = "Hailar", ProvinceCode = "NM", CurrencyCode = "CNY" },
                new Destination { DestinationID = 12, City = "Hangzhou", ProvinceCode = "ZJ", CurrencyCode = "CNY" },
                new Destination { DestinationID = 13, City = "Harbin", ProvinceCode = "HL", CurrencyCode = "CNY" },
                new Destination { DestinationID = 14, City = "Hefei", ProvinceCode = "AH", CurrencyCode = "CNY" },
                new Destination { DestinationID = 15, City = "Hohhot", ProvinceCode = "NM", CurrencyCode = "CNY" },
                new Destination { DestinationID = 16, City = "Hong Kong", ProvinceCode = "HK", CurrencyCode = "HKD" },
                new Destination { DestinationID = 17, City = "Hulun Buir", ProvinceCode = "NM", CurrencyCode = "CNY" },
                new Destination { DestinationID = 18, City = "Jinan", ProvinceCode = "SD", CurrencyCode = "CNY" },
                new Destination { DestinationID = 19, City = "Kashi", ProvinceCode = "XJ", CurrencyCode = "CNY" },
                new Destination { DestinationID = 20, City = "Kunming", ProvinceCode = "YN", CurrencyCode = "CNY" },
                new Destination { DestinationID = 21, City = "Lanzhou", ProvinceCode = "GS", CurrencyCode = "CNY" },
                new Destination { DestinationID = 22, City = "Lhasa", ProvinceCode = "XZ", CurrencyCode = "CNY" },
                new Destination { DestinationID = 23, City = "Macau", ProvinceCode = "MC", CurrencyCode = "MOP" },
                new Destination { DestinationID = 24, City = "Nanchang", ProvinceCode = "JX", CurrencyCode = "CNY" },
                new Destination { DestinationID = 25, City = "Nanjing", ProvinceCode = "JS", CurrencyCode = "CNY" },
                new Destination { DestinationID = 26, City = "Nanning", ProvinceCode = "JX", CurrencyCode = "CNY" },
                new Destination { DestinationID = 27, City = "Qiqihar", ProvinceCode = "HL", CurrencyCode = "CNY" },
                new Destination { DestinationID = 28, City = "Shanghai", ProvinceCode = "SH", CurrencyCode = "CNY" },
                new Destination { DestinationID = 29, City = "Shenyang", ProvinceCode = "LN", CurrencyCode = "CNY" },
                new Destination { DestinationID = 30, City = "Shijiazhuang", ProvinceCode = "HE", CurrencyCode = "CNY" },
                new Destination { DestinationID = 31, City = "Taipei", ProvinceCode = "TW", CurrencyCode = "TWD" },
                new Destination { DestinationID = 32, City = "Taiyuan", ProvinceCode = "SX", CurrencyCode = "CNY" },
                new Destination { DestinationID = 33, City = "Tianjin", ProvinceCode = "HE", CurrencyCode = "CNY" },
                new Destination { DestinationID = 34, City = "Urumqi", ProvinceCode = "XJ", CurrencyCode = "CNY" },
                new Destination { DestinationID = 35, City = "Wuhan", ProvinceCode = "HB", CurrencyCode = "CNY" },
                new Destination { DestinationID = 36, City = "Xi'an", ProvinceCode = "SN", CurrencyCode = "CNY" },
                new Destination { DestinationID = 37, City = "Xining", ProvinceCode = "QH", CurrencyCode = "CNY" },
                new Destination { DestinationID = 38, City = "Yinchuan", ProvinceCode = "NX", CurrencyCode = "CNY" },
                new Destination { DestinationID = 39, City = "Yumen", ProvinceCode = "GS", CurrencyCode = "CNY" },
                new Destination { DestinationID = 40, City = "Zhengzhou", ProvinceCode = "HA", CurrencyCode = "CNY" }
                );

            // Add currency data.
            context.Currencies.AddOrUpdate(
                p => p.CurrencyCode,
                new Currency { CurrencyCode = "CNY", ExchangeRate = 1.00M },
                new Currency { CurrencyCode = "HKD", ExchangeRate = 1.13M },
                new Currency { CurrencyCode = "MOP", ExchangeRate = 1.16M },
                new Currency { CurrencyCode = "TWD", ExchangeRate = 4.56M }
                );


            //Add package type size data
            context.PackageTypeSizes.AddOrUpdate(
                p => p.PackageTypeSizeID,
                new PackageTypeSize { PackageTypeSizeID = 1, TypeSize = "250*350mm", WeightLimit = "Not Applicable", PackageTypeID = 1 },
                new PackageTypeSize { PackageTypeSizeID = 2, TypeSize = "small - 350*400mm", WeightLimit = "5kg", PackageTypeID = 2 },
                new PackageTypeSize { PackageTypeSizeID = 3, TypeSize = "large - 450*550mm", WeightLimit = "5kg", PackageTypeID = 2 },
                new PackageTypeSize { PackageTypeSizeID = 4, TypeSize = "1000*80mm", WeightLimit = "Not Applicable", PackageTypeID = 3 },
                new PackageTypeSize { PackageTypeSizeID = 5, TypeSize = "small - 300*250*150mm", WeightLimit = "10kg", PackageTypeID = 4 },
                new PackageTypeSize { PackageTypeSizeID = 6, TypeSize = "medium - 400*350*250mm", WeightLimit = "20kg", PackageTypeID = 4 },
                new PackageTypeSize { PackageTypeSizeID = 7, TypeSize = "large - 500*450*350mm", WeightLimit = "30kg", PackageTypeID = 4 },
                new PackageTypeSize { PackageTypeSizeID = 36, TypeSize = "custom", WeightLimit = "Not Applicable", PackageTypeID = 5 }

                );

            // Add penalty.
            context.Penalties.AddOrUpdate(
                p => p.PenaltyID,
                new Penalty { PenaltyID = 1, PenaltyCharge = 500M }
                );

            // Add shipment data.
            /*
            context.Shipments.AddOrUpdate(
                p => p.WaybillId,
                new Shipment { WaybillId = 1, ReferenceNumber = "", ServiceType = "Same Day", ShippedDate = new DateTime(2016, 11, 11), DeliveredDate = new DateTime(2016, 11, 11), RecipientName = "Andy Ho", NumberOfPackages = 1, Origin = "Hong Kong", Destination = "Guangzhou", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 2, ReferenceNumber = "A28756", ServiceType = "Same Day", ShippedDate = new DateTime(2016, 12, 12), DeliveredDate = new DateTime(2016, 12, 12), RecipientName = "John Wong", NumberOfPackages = 2, Origin = "Hong Kong", Destination = "Macau", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 3, ReferenceNumber = "", ServiceType = "Next Day 10:30", ShippedDate = new DateTime(2016, 12, 31), DeliveredDate = new DateTime(2017, 01, 01), RecipientName = "John Wong", NumberOfPackages = 1, Origin = "Hong Kong", Destination = "Beijing", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 4, ReferenceNumber = "", ServiceType = "Next Day 10:30", ShippedDate = new DateTime(2016, 11, 30), DeliveredDate = new DateTime(2016, 12, 01), RecipientName = "Daisy Chan", NumberOfPackages = 3, Origin = "Hong Kong", Destination = "Shanghai", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 5, ReferenceNumber = "", ServiceType = "Next Day 12:00", ShippedDate = new DateTime(2016, 11, 17), DeliveredDate = new DateTime(2016, 11, 18), RecipientName = "Daisy Chan", NumberOfPackages = 1, Origin = "Hong Kong", Destination = "Kashi", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 6, ReferenceNumber = "", ServiceType = "Ground", ShippedDate = new DateTime(2016, 12, 16), DeliveredDate = new DateTime(2016, 12, 15), RecipientName = "Harry Lee", NumberOfPackages = 1, Origin = "Hong Kong", Destination = "Harbin", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 7, ReferenceNumber = "45236", ServiceType = "2nd Day", ShippedDate = new DateTime(2017, 01, 14), DeliveredDate = new DateTime(2017, 01, 16), RecipientName = "John Wong", NumberOfPackages = 2, Origin = "Hong Kong", Destination = "Changchun", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 8, ReferenceNumber = "", ServiceType = "Next Day", ShippedDate = new DateTime(2016, 10, 19), DeliveredDate = new DateTime(2016, 10, 20), RecipientName = "Lisa Li", NumberOfPackages = 1, Origin = "Beijing", Destination = "Haikou", Status = "Delivered", ShippingAccountId = 2 },
                new Shipment { WaybillId = 9, ReferenceNumber = "", ServiceType = "Same Day", ShippedDate = new DateTime(2016, 11, 04), DeliveredDate = new DateTime(2016, 11, 05), RecipientName = "Yolanda Yu", NumberOfPackages = 1, Origin = "Beijing", Destination = "Hangzhou", Status = "Delivered", ShippingAccountId = 2 },
                new Shipment { WaybillId = 10, ReferenceNumber = "", ServiceType = "Next Day", ShippedDate = new DateTime(2017, 02, 02), DeliveredDate = new DateTime(2017, 02, 03), RecipientName = "Yolanda Yu", NumberOfPackages = 2, Origin = "Beijing", Destination = "Jinan", Status = "Delivered", ShippingAccountId = 2 },
                new Shipment { WaybillId = 11, ReferenceNumber = "66543", ServiceType = "Ground", ShippedDate = new DateTime(2017, 01, 23), DeliveredDate = new DateTime(2017, 01, 26), RecipientName = "Arnold Au", NumberOfPackages = 3, Origin = "Beijing", Destination = "Guangzhou", Status = "Delivered", ShippingAccountId = 2 },
                new Shipment { WaybillId = 15, ReferenceNumber = "", ServiceType = "Ground", ShippedDate = new DateTime(2017, 01, 13), DeliveredDate = new DateTime(2017, 01, 20), RecipientName = "John Wong", NumberOfPackages = 1, Origin = "Hong Kong", Destination = "Nanning", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 17, ReferenceNumber = "", ServiceType = "2nd Day", ShippedDate = new DateTime(2017, 02, 10), DeliveredDate = new DateTime(2017, 02, 12), RecipientName = "Kelly Kwong", NumberOfPackages = 6, Origin = "Hong Kong", Destination = "Golmud", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 18, ReferenceNumber = "", ServiceType = "Same Day", ShippedDate = new DateTime(2017, 01, 18), DeliveredDate = new DateTime(2017, 01, 18), RecipientName = "Wendy Wang", NumberOfPackages = 4, Origin = "Hong Kong", Destination = "Hohhot", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 19, ReferenceNumber = "", ServiceType = "2nd Day", ShippedDate = new DateTime(2017, 02, 06), DeliveredDate = new DateTime(2017, 02, 08), RecipientName = "Larry Leung", NumberOfPackages = 2, Origin = "Guangzhou", Destination = "Hong Kong", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 20, ReferenceNumber = "22233398", ServiceType = "Next Day 15:00", ShippedDate = new DateTime(2016, 10, 09), DeliveredDate = new DateTime(2016, 10, 10), RecipientName = "Larry Leung", NumberOfPackages = 1, Origin = "Beijing", Destination = "Hong Kong", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 23, ReferenceNumber = "", ServiceType = "Next Day 15:00", ShippedDate = new DateTime(2016, 10, 23), DeliveredDate = new DateTime(2016, 10, 24), RecipientName = "Harry Ho", NumberOfPackages = 2, Origin = "Hefei", Destination = "Beijing", Status = "Delivered", ShippingAccountId = 1 },
                new Shipment { WaybillId = 24, ReferenceNumber = "", ServiceType = "Ground", ShippedDate = new DateTime(2017, 01, 15), DeliveredDate = new DateTime(2017, 01, 19), RecipientName = "Peter Pang", NumberOfPackages = 3, Origin = "Beijing", Destination = "Lhasa", Status = "Delivered", ShippingAccountId = 2 },
                new Shipment { WaybillId = 25, ReferenceNumber = "386456", ServiceType = "Same Day", ShippedDate = new DateTime(2017, 01, 05), DeliveredDate = new DateTime(2017, 01, 05), RecipientName = "Jerry Jia", NumberOfPackages = 1, Origin = "Beijing", Destination = "Hangzhou", Status = "Delivered", ShippingAccountId = 2 }
            );
            */
            // han er leng zi
            // Add recipient data.


            //Add pickup loaction 


        }
    }
}
