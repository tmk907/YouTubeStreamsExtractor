using Shouldly;

namespace YouTubeStreamsExtractor.Tests
{
    public class CacheTests
    {
        [Test]
        public void test_CacheGetOrAdd_should_not_add_new_value_if_key_exists()
        {
            var cache = new Cache();

            var value1 = cache.GetOrAdd("key1", () => "value1");
            var value2 = cache.GetOrAdd("key1", () => "value2");

            value1.ShouldNotBe("value2");
            value2.ShouldBe("value1");
        }

        [Test]
        public void test_CacheTryGetValue()
        {
            var cache = new Cache();

            cache.Add("key1", "value1");

            var hasValue1 = cache.TryGetValue("key1", out string value1);
            var hasValue2 = cache.TryGetValue("key2", out string value2);

            hasValue1.ShouldBe(true);
            hasValue2.ShouldBe(false);

            value1.ShouldBe("value1");
            value2.ShouldBe(null);
        }

        [Test]
        public void test_CacheGetOrAdd_does_not_store_empty_string()
        {
            var cache = new Cache();

            var value1 = cache.GetOrAdd("key1", () => "");
            var value2 = cache.GetOrAdd("key2", () => "value2");

            value2.ShouldBe("value2");
        }
    }
}
