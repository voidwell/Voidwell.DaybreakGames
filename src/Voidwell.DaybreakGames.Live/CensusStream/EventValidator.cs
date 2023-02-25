using AsyncKeyedLock;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public class EventValidator : IEventValidator
    {
        private readonly ConcurrentDictionary<string, object> _eventBuffer = new ConcurrentDictionary<string, object>();
        private readonly AsyncKeyedLocker<string> _asyncKeyedLocker;

        public EventValidator(AsyncKeyedLocker<string> asyncKeyedLocker)
        {
            _asyncKeyedLocker = asyncKeyedLocker;
        }

        public async Task<bool> Validiate<T>(T ev, Func<T, string> keyExpr, Func<T, bool> cleanupExpr) where T : class
        {
            var eventKey = $"{typeof(T).Name}:{keyExpr(ev)}";
            using (await _asyncKeyedLocker.LockAsync(eventKey).ConfigureAwait(false))
            {
                var isValid = !_eventBuffer.ContainsKey(eventKey);

                var expiredKeys = _eventBuffer.Keys.ToList()
                    .Where(k => _eventBuffer.TryGetValue(k, out var value) && value is T && cleanupExpr(value as T)).ToList();
                expiredKeys.ForEach(k => _eventBuffer.TryRemove(k, out var value));

                _eventBuffer.TryAdd(eventKey, ev);

                return isValid;
            }
        }
    }
}
