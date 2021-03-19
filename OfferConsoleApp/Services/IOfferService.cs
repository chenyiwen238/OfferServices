using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OfferWebApiClientConsoleApp.Models;
using System.Net.Http;

namespace OfferWebApiClientConsoleApp.Services
{
    public interface IOfferService
    {
        Task<IReadOnlyCollection<IOfferDto>> GetAllOffers(IReadOnlyCollection<HttpRequestMessage> requestMessages);
        Task<IOfferDto> GetOfferWithLowestAmount(IReadOnlyCollection<IOfferDto> offers);
    }
}
