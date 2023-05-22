using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Defra.Trade.ReMoS.AssuranceService.UI.Core.UnitTests.Integration
{
    public class MockMessageHandler : HttpMessageHandler
    {
        private readonly Uri _requestUri;
        private readonly string _result;
        private bool _trueSend;
        private HttpStatusCode _statusCode;

        public MockMessageHandler(Uri requestUri, string result, bool trueSend = false, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _requestUri = requestUri;
            _result = result;
            _trueSend = trueSend;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;

            if (!_trueSend)
            {
                response = new HttpResponseMessage
                {
                    StatusCode = _statusCode,
                    Content = new StringContent(_result)
                };

                return Task.FromResult(response);
            }

            if (request.RequestUri == _requestUri)
            {
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(_result)
                };
            }
            else
            {
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            return Task.FromResult(response);
        }
    }
}
