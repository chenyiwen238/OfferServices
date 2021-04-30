using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfferWebApiClientConsoleApp.Models;
using OfferWebApiClientConsoleApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfferWebApiClientConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //new host builder
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureServices((hostcontext, services) =>
                {
                    services.AddHttpClient();
                    services.AddScoped<IOfferService, OfferService>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                }); 

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var offerService = services.GetRequiredService<IOfferService>();
                    
                    ICollection<string> uris = SetUpUri();
                    IReadOnlyCollection<HttpRequestMessage> requestMessages = ConsolidateRequestMessages(uris ?? null);
                    var allOffers = await offerService.GetAllOffers(requestMessages);
                    IOfferDto bestDeal = await offerService.GetOfferWithLowestAmount(allOffers);

                    Console.WriteLine
                        ($"The best offer comes from source {0}, destination {1}, amount of {2}"
                        ,bestDeal?.SourceAddress
                        ,bestDeal?.DestinationAddress
                        ,bestDeal?.OfferAmount
                        );
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error Occured.");
                }
            }
        }

        #region Utility Methods

        /// <summary>
        /// Method which generate a collection of uris.
        /// Ideally this should get from config files or settings.
        /// </summary>
        /// <returns>A collection of uris.</returns>
        private static ICollection<string> SetUpUri()
        {
            string baseaddress = "https://www.baseaddress.com/api/offers/";
            string apiSignature1 = JsonConvert.SerializeObject(offerInput1);
            string apiSignature2 = JsonConvert.SerializeObject(offerInput2);
            //TODO:
            //string apiSignature3 = SerializeObjectToXml(offerInput3);
            string apiSignature3 = JsonConvert.SerializeObject(offerInput3);

            ICollection<string> uris = new List<string>
                    { baseaddress + apiSignature1,
                      baseaddress + apiSignature2,
                      baseaddress + apiSignature3
                    };

            return uris;
        }

        /// <summary>
        /// Create Request message from a collection of uri's.
        /// </summary>
        /// <param name="uris">Collection of uris.</param>
        /// <returns>A collection of HttpRequestMessage.</returns>
        private static IReadOnlyCollection<HttpRequestMessage> ConsolidateRequestMessages(ICollection<string> uris)
        {
            return uris?.Select(uri => new HttpRequestMessage(HttpMethod.Get, uri)).ToList();
        }

        private static string SerializeObjectToXml(OfferDto offer)
        {
            Stream stream = new MemoryStream();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(OfferDto));
            x.Serialize(stream, offer);
            var outputString = stream.ToString();
            stream.Close();
            return outputString;
        }

        #endregion

        #region sample data

        /*Sample data set up*/
        static IOfferDtoJson offerInput1 = new OfferDto()
        {
            ContactAddress = "Source Address 1",
            WarehouseAddress = "Destination Address 1",
            PackageDimensions = new List<Tuple<double, double, double>>() { new Tuple<double, double, double>(4, 15, 10), new Tuple<double, double, double>(12, 3, 7.5) }
        };

        static IOfferDtoJson offerInput2 = new OfferDto()
        {
            Consignee = "Source Address 2",
            Consignor = "Destination Address 2",
            Cartons = new List<Tuple<double, double, double>>() { new Tuple<double, double, double>(4, 15, 10) }
        };

        static IOfferDto offerInput3 = new OfferDto()
        {
            SourceAddress = "Source Address 3",
            DestinationAddress = "Destination Address 3"
        };

        #endregion


    }
}
