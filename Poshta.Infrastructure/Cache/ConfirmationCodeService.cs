using Microsoft.Extensions.Caching.Memory;

namespace Poshta.Infrastructure.Cache
{
    public class ConfirmationCodeService(IMemoryCache cache) : Application.Auth.IConfirmationCodeService
    {
        private readonly IMemoryCache cache = cache;
        private const int CodeExpirationInMinutes = 1;

        public Task<string> GenerateCodeAsync(string key)
        {
            var code = new Random().Next(1000, 9999).ToString();
            cache.Set(key, code, TimeSpan.FromMinutes(CodeExpirationInMinutes));
            return Task.FromResult(code);
        }

        public Task<bool> ValidateCodeAsync(string key, string code)
        {
            if (cache.TryGetValue(key, out string? cachedCode))
            {
                return Task.FromResult(cachedCode == code);
            }

            return Task.FromResult(false);
        }

        public void RemoveCode(string key)
        {
            cache.Remove(key);
        }
    }
}
