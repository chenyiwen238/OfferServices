using OfferWebApiClientConsoleApp.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OfferWebApiClientConsoleApp.Services
{
    /// <summary>
    /// Service class contains methods related to Offer operations.
    /// </summary>
    public class OfferService : IOfferService
    {
        private readonly IHttpClientFactory _clientFactory;

        public OfferService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// <summary>
        /// Request multiple API concurrently and convert output as a collection of OfferDto's.
        /// </summary>
        /// <param name="requestMessages">Collection of RequestMessages</param>
        /// <returns>Collection of OfferDto</returns>
        public async Task<IReadOnlyCollection<IOfferDto>> GetAllOffers(IReadOnlyCollection<HttpRequestMessage> requestMessages)
        {
            if (requestMessages == null || requestMessages.Count == 0)
                return new List<OfferDto>();

            var responses = new ConcurrentBag<IReadOnlyCollection<OfferDto>>();

            //Use SemaphoreSlim to control the number of threads that can access a resource/pool of resources concurrently
            var semaphore = new SemaphoreSlim(initialCount:3,maxCount:10);

            //My attemp is to request all API's, covert the outputs to OfferDto's,
            //and save these OfferDto's concurrently
            var tasks = requestMessages.Select(async requestMessage =>
            {
                await semaphore.WaitAsync();
                try 
                {
                    var client = _clientFactory.CreateClient();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.SendAsync(requestMessage);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var offers = await response.Content?.ReadAsAsync<IReadOnlyCollection<OfferDto>>();

                        if(offers != null)
                            responses.Add(offers);
                    }
                }
                finally 
                {
                    semaphore.Release();
                }
            });
            
            await Task.WhenAll(tasks);
            return responses.SelectMany(x => x).ToList();
        }

        /// <summary>
        /// Get the offer with the lowest amount.
        /// </summary>
        /// <param name="offers">Collection of offers.</param>
        /// <returns>The offer has the lowest amount</returns>
        public Task<IOfferDto> GetOfferWithLowestAmount(IReadOnlyCollection<IOfferDto> offers)
        {
            return Task.Run(() =>
            {
                if (offers is null || offers.Count == 0)
                    return null;

                var lowestOfferAmount =
                offers.Aggregate((minItem, nextItem) => minItem.OfferAmount < nextItem.OfferAmount ? minItem : nextItem);
                //Vs.
                //var lowestOfferAmount =
                //offers.AsParallel().Aggregate((minItem, nextItem) => minItem.OfferAmount < nextItem.OfferAmount ? minItem : nextItem);

                return lowestOfferAmount;
            });
        }
    }
}
