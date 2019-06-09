using System.Net.Http;

namespace Voidwell.DaybreakGames.HttpAuthenticatedClient
{
    public interface IAuthenticatedHttpClientFactory
    {
        HttpClient GetHttpClient();
    }
}
