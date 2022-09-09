using System.Collections.Concurrent;

namespace YouTubeStreamsExtractor
{
    public interface ICache
    {
        void Add(string key, string value);
        void Clear();
        string GetOrAdd(string key, Func<string> valueFactory);
        Task<string> GetOrAddAsync(string key, Func<Task<string>> valueFactory);
        bool TryGetValue(string key, out string value);
    }

    public class Cache : ICache
    {
        private readonly ConcurrentDictionary<string, string> _cache;

        public Cache()
        {
            _cache = new ConcurrentDictionary<string, string>();
        }

        public void Add(string key, string value)
        {
            if (value == "") return;
            _cache.TryAdd(key, value);
        }

        public string GetOrAdd(string key, Func<string> valueFactory)
        {
            if (_cache.TryGetValue(key, out string value) && !string.IsNullOrEmpty(value))
            {
                return value;
            }
            value = valueFactory();
            _cache.TryAdd(key, value);
            return value;
        }

        public async Task<string> GetOrAddAsync(string key, Func<Task<string>> valueFactory)
        {
            if (_cache.TryGetValue(key, out string value) && !string.IsNullOrEmpty(value))
            {
                return value;
            }
            value = await valueFactory();
            _cache.TryAdd(key, value);
            return value;
        }

        public bool TryGetValue(string key, out string value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
