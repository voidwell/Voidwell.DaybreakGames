using System.Collections.Generic;
using System.Net.Http;

namespace Voidwell.DaybreakGames.HttpAuthenticatedClient
{
    public class AuthenticatedHttpClientOptions
    {
        public AuthenticatedHttpClientOptions()
        {
            Scopes = new List<string>();
        }

        public string TokenServiceAddress { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public List<string> Scopes { get; set; }

        public HttpMessageHandler InnerMessageHandler { get; set; }
        public HttpMessageHandler TokenServiceMessageHandler { get; set; }

        /// <summary>
        /// The number of milliseconds the token server has to respond before a timeout
        /// </summary>
        public int TokenServiceTimeout { get; set; } = 10000;
        /// <summary>
        /// The number of milliseconds a message has to be handled (including getting the access token) before a timeout
        /// </summary>
        public int MessageHandlerTimeout { get; set; } = 60000;
    }
}
