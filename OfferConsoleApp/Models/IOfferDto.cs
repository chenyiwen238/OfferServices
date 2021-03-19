using System;
using System.Collections.Generic;

namespace OfferWebApiClientConsoleApp.Models
{
    public interface IOfferDto
    {
        string SourceAddress { get; set; }
        string DestinationAddress { get; set; }
        double OfferAmount { get; set; }

        ICollection<Tuple<double, double, double>> CartonDimensions { get; set; }
    }
}
