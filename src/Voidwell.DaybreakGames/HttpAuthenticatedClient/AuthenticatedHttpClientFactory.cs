using System;
using System.Net.Http;

namespace Voidwell.DaybreakGames.HttpAuthenticatedClient
{
    public class AuthenticatedHttpClientFactory : IAuthenticatedHttpClientFactory
    {
        private readonly Func<AuthenticatedHttpMessageHandler> _authenticatedHttpMessageHandlerFactory;

        public AuthenticatedHttpClientFactory(Func<AuthenticatedHttpMessageHandler> authenticatedHttpMessageHandlerFactory)
        {
            _authenticatedHttpMessageHandlerFactory = authenticatedHttpMessageHandlerFactory;
        }

        public HttpClient GetHttpClient()
        {
            return new HttpClient(_authenticatedHttpMessageHandlerFactory(), true);
        }
    }
}
