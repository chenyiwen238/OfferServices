using System;
using System.Collections.Generic;

namespace OfferWebApiClientConsoleApp.Models
{
    public interface IOfferDtoJson
    {
        string ContactAddress { get; set; }
        string Consignee { get; set; }
        string WarehouseAddress { get; set; }
        string Consignor { get; set; }

        ICollection<Tuple<double, double, double>> PackageDimensions { get; set; }
        ICollection<Tuple<double, double, double>> Cartons { get; set; }

        double Total { get; set; }
        double Amount { get; set; }

    }
}
