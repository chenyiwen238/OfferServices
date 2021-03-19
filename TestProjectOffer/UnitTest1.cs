using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using OfferWebApiClientConsoleApp.Models;
using OfferWebApiClientConsoleApp.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace TestProjectOffer
{
    public class UnitTest1
    {
        #region Test Data
        static string mockUri = "https://www.baseaddress.com/api/offers";

        IOfferDto offer1 = new OfferDto()
        {
            SourceAddress = "Source Address 1",
            DestinationAddress = "Destination Address 1",
            OfferAmount = 73.28,
            CartonDimensions = new List<Tuple<double, double, double>>() { new Tuple<double, double, double>(4, 15, 10), new Tuple<double, double, double>(12, 3, 7.5) }
        };

        IOfferDto offer2 = new OfferDto()
        {
            SourceAddress = "Source Address 2",
            DestinationAddress = "Destination Address 2",
            OfferAmount = 30.12,
            CartonDimensions = new List<Tuple<double, double, double>>() { new Tuple<double, double, double>(4, 15, 10) }
        };

        IOfferDto offer3 = new OfferDto()
        {
            SourceAddress = "Source Address 3",
            DestinationAddress = "Destination Address 3",
            OfferAmount = 54.37,
            CartonDimensions = new List<Tuple<double, double, double>>() { new Tuple<double, double, double>(36, 18, 24) }
        };

        #endregion

        [Fact]
        public async void WhenRequestMessagesProvided_ServiceShouldReturnAllOffers()
        {
            //Arrange
            var offerDtos = new List<IOfferDto>() { offer1, offer2, offer3 };
            
            var mockRequestMessage = new HttpRequestMessage(HttpMethod.Get, mockUri);
            var mockHttpRequestMessages = new List<HttpRequestMessage> { mockRequestMessage };
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            
            var mockHttpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(offerDtos), Encoding.UTF8, "application/json")
                
            });

            var fakeHttpClient = new HttpClient(mockHttpMessageHandler);
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            //Act
            var service = new OfferService(httpClientFactoryMock);
            var result = await service.GetAllOffers(mockHttpRequestMessages);

            //Assert
            result
            .Should()
            .BeOfType<List<OfferDto>>()
            .And
            .HaveCount(3)
            .And
            .Contain(x => x.OfferAmount == 73.28 && x.SourceAddress.Equals("Source Address 1"))
            .And
            .Contain(x => x.OfferAmount == 30.12 && x.SourceAddress.Equals("Source Address 2"))
            .And
            .Contain(x => x.OfferAmount == 54.37 && x.SourceAddress.Equals("Source Address 3"));
        }

        [Fact]
        public async void WhenRequestMessagesProvided_AndReturnXmlConent_ServiceShouldReturnOffers()
        {
            //Arrange
            string xmlContent = @"<xml><source>Source Address 1</source><destination /><quote>73.28</quote><packages><package/></packages></xml>";
            var mockRequestMessage = new HttpRequestMessage(HttpMethod.Get, mockUri);
            var mockHttpRequestMessages = new List<HttpRequestMessage> { mockRequestMessage };
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();

            var mockHttpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(xmlContent, Encoding.UTF8, "application/xml")
            });

            var fakeHttpClient = new HttpClient(mockHttpMessageHandler);
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            //Act
            var service = new OfferService(httpClientFactoryMock);
            var result = await service.GetAllOffers(mockHttpRequestMessages);

            //Assert
            result
            .Should()
            .BeOfType<List<OfferDto>>()
            .And
            .HaveCount(1)
            .And
            .Contain(x => x.OfferAmount == 73.28 && x.SourceAddress.Equals("Source Address 1"));
        }

        [Fact]
        public async void WhenNoRequestMessagesProvided_ServiceShouldReturnNoResult()
        {
            //Arrange
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();

            var mockHttpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            var fakeHttpClient = new HttpClient(mockHttpMessageHandler);
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            //Act
            var service = new OfferService(httpClientFactoryMock);
            var result = await service.GetAllOffers(null);

            //Assert
            result
            .Should()
            .BeOfType<List<OfferDto>>()
            .And
            .HaveCount(0);
        }

        [Fact]
        public async void WhenRequestMessagesProvidedButNoOutputReturn_ServiceShouldReturnNoResult()
        {
            //Arrange
            var offerDtos = new List<OfferDto>();
            var mockRequestMessage = new HttpRequestMessage(HttpMethod.Get, mockUri);
            var mockHttpRequestMessages = new List<HttpRequestMessage> { mockRequestMessage };
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();

            var mockHttpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(offerDtos), Encoding.UTF8, "application/json")
            });

            var fakeHttpClient = new HttpClient(mockHttpMessageHandler);
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            //Act
            var service = new OfferService(httpClientFactoryMock);
            var result = await service.GetAllOffers(mockHttpRequestMessages);

            //Assert
            result
            .Should()
            .BeOfType<List<OfferDto>>()
            .And
            .HaveCount(0);
        }

        [Fact]
        public void WhenCollectionOfOffersProvided_ServiceShouldReturnOfferHasLowestAmount()
        {
            //Arrange
            var offerDtos = new List<IOfferDto>() { offer1, offer2, offer3 };
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();

            var fakeHttpClient = new HttpClient();
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            //Act
            var service = new OfferService(httpClientFactoryMock);
            var result = service.GetOfferWithLowestAmount(offerDtos).Result;

            //Assert
            result
            .Should()
            .BeOfType<OfferDto>()
            .Which.OfferAmount.Equals(30.12);
        }

        [Fact]
        public async void WhenCollectionOfOffersIsEmptyProvided_ServiceShouldReturnNull()
        {
            //Arrange
            var offerDtos = new List<OfferDto>();
            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();

            var fakeHttpClient = new HttpClient();
            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            //Act
            var service = new OfferService(httpClientFactoryMock);
            var result = await service.GetOfferWithLowestAmount(offerDtos);

            //Assert
            result
            .Should()
            .BeNull();
        }

    }
}
