using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace TestProjectOffer
{
    internal class MockHttpMessageHandler:DelegatingHandler
    {
        private HttpResponseMessage _mockhttpResponseMessage;

        public MockHttpMessageHandler(HttpResponseMessage httpResponseMessage)
        {
            this._mockhttpResponseMessage = httpResponseMessage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_mockhttpResponseMessage);
        }
    }
}