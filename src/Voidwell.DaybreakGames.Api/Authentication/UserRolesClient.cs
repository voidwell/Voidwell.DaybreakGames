using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Api.Authentication
{
    public class UserRolesClient : IUserRolesClient
    {
        private readonly HttpClient _httpClient;

        public UserRolesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<string>> GetRolesAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"roles/{userId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<string>>();
        }
    }
}
