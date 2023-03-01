using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Microservice.Utility;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public class EventValidator : IEventValidator, IDisposable
    {
        private readonly ConcurrentDictionary<string, object> _eventBuffer = new ConcurrentDictionary<string, object>();
        private readonly KeyedSemaphoreSlim _eventSemaphore = new KeyedSemaphoreSlim();

        public async Task<bool> Validiate<T>(T ev, Func<T, string> keyExpr, Func<T, bool> cleanupExpr) where T : class
        {
            var eventKey = $"{typeof(T).Name}:{keyExpr(ev)}";
            using (await _eventSemaphore.WaitAsync(eventKey))
            {
                var isValid = !_eventBuffer.ContainsKey(eventKey);

                var expiredKeys = _eventBuffer.Keys.ToList()
                    .Where(k => _eventBuffer.TryGetValue(k, out var value) && value is T && cleanupExpr(value as T)).ToList();
                expiredKeys.ForEach(k => _eventBuffer.TryRemove(k, out var value));

                _eventBuffer.TryAdd(eventKey, ev);

                return isValid;
            }
        }

        public void Dispose()
        {
            _eventSemaphore.Dispose();
        }
    }
}
