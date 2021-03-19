using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OfferWebApiClientConsoleApp.Models
{
    public class OfferDto: IOfferDto, IOfferDtoJson
    {
        //[XmlElement(ElementName = "source")]
        public string SourceAddress { get; set; }

        //[XmlElement(ElementName = "destination")]
        public string DestinationAddress { get; set; }
        
        //[XmlIgnore]
        public ICollection<Tuple<double, double, double>> CartonDimensions { get; set; }

        //[XmlElement(ElementName = "quote")]
        public double OfferAmount { get; set; }

        [XmlIgnore]
        [JsonProperty("contact address")]
        public string ContactAddress 
        { 
            get { return SourceAddress; }
            set { SourceAddress = value; } 
        }

        [XmlIgnore]
        [JsonProperty("consignee")]
        public string Consignee 
        { 
            get { return SourceAddress; } 
            set { SourceAddress = value; } 
        }

        [XmlIgnore]
        [JsonProperty("warehouse address")]
        public string WarehouseAddress 
        { 
            get { return DestinationAddress; } 
            set { DestinationAddress = value; } 
        }

        [XmlIgnore]
        [JsonProperty("consignor")]
        public string Consignor 
        { 
            get { return DestinationAddress;  }
            set { DestinationAddress = value; } 
        }

        [XmlIgnore]
        [JsonProperty("package dimensions")]
        public ICollection<Tuple<double, double, double>> PackageDimensions
        {
            get { return CartonDimensions; }
            set { CartonDimensions = value; } 
        }

        [XmlIgnore]
        [JsonProperty("cartons")]
        public ICollection<Tuple<double, double, double>> Cartons 
        {
            get { return CartonDimensions; }
            set { CartonDimensions = value; } 
        }

        [XmlIgnore]
        [JsonProperty("total")]
        public double Total 
        {
            get { return OfferAmount; }
            set { OfferAmount = value; } 
        }

        [XmlIgnore]
        [JsonProperty("amount")]
        public double Amount 
        { get { return OfferAmount; }
            set { OfferAmount = value; } 
        }
    }
}
