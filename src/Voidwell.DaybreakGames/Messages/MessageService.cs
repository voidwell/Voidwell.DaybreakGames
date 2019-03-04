using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.HttpAuthenticatedClient;
using Voidwell.DaybreakGames.Messages.Models;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Messages
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly DaybreakGamesOptions _options;

        private readonly IZoneService _zoneService;
        private readonly IWorldService _worldService;

        public MessageService(IAuthenticatedHttpClientFactory authenticatedHttpClientFactory, IZoneService zoneService, IWorldService worldService, IOptions<DaybreakGamesOptions> options)
        {
            _httpClient = authenticatedHttpClientFactory.GetHttpClient();
            _httpClient.BaseAddress = new Uri("http://messagewell:5000");

            _options = options.Value;
            _zoneService = zoneService;
            _worldService = worldService;
        }

        public async Task PublishCharacterEvent<T>(string characterId, T message) where T : PlanetsideCharacterMessage
        {
            message = await EnrichMessage(message);

            RunPublishAsync("ps2", "character", characterId, message);
        }

        public async Task PublishAlertEvent<T>(int worldId, int instanceId, T message) where T : PlanetsideAlertMessage
        {
            message = await EnrichMessage(message);

            var key = $"{worldId}-{instanceId}";

            RunPublishAsync("ps2", "alert", worldId.ToString(), message);
            RunPublishAsync("ps2", "alert", key, message);
            RunPublishAsync("ps2", "alert", message);
        }

        private async Task<T> EnrichMessage<T>(T message) where T : PlanetsideMessage
        {
            var worldTask = _worldService.GetWorld(message.WorldId);
            Task<Zone> zoneTask = null;

            List<Task> tasks = new List<Task>{
                worldTask
            };

            if (message.ZoneId.HasValue)
            {
                zoneTask = _zoneService.GetZone(message.ZoneId.Value);
                tasks.Add(zoneTask);
            }

            await Task.WhenAll(tasks);

            message.WorldName = worldTask?.Result?.Name;
            message.ZoneName = zoneTask?.Result?.Name;

            return message;
        }

        private void RunPublishAsync<T>(string category, string channel, T message) where T : PlanetsideMessage
        {
            RunPublishAsync<T>(category, channel, null, message);
        }

        private void RunPublishAsync<T>(string category, string channel, string id, T message) where T : PlanetsideMessage
        {
            Task.Run(() => PublishAsync(category, channel, id, message));
        }

        private Task PublishAsync<T>(string category, string channel, string id, T message) where T : PlanetsideMessage
        {
            if (_options.DisableMessages)
            {
                return Task.CompletedTask;
            }

            var content = JsonContent.FromObject(message);
            var pubKey = $"{category}/{channel}";
            if (!string.IsNullOrWhiteSpace(id)) {
                pubKey += $"/{id}";
            }
            return _httpClient.PostAsync($"publish/{pubKey}", content);
        }
    }
}
