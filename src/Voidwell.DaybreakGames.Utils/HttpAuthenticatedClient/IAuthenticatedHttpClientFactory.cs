using System.Net.Http;

namespace Voidwell.DaybreakGames.Utils.HttpAuthenticatedClient
{
    public interface IAuthenticatedHttpClientFactory
    {
        HttpClient GetHttpClient();
    }
}
