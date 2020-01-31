using horoscope_crawler.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace horoscope_crawler.Services
{
    public class HoroscopeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;

        private static readonly string[] _months = { "ocak", "subat", "mart", "nisan", "mayis", "haziran", "temmuz", "agustos", "eylul", "ekim", "kasim", "aralik" };
        private static readonly string[] _horizons = { "koc", "boga", "ikizler", "yengec", "aslan", "basak", "terazi", "akrep", "yay", "oglak", "kova", "balik" };
        public HoroscopeService(IHttpClientFactory httpClientFactory, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<IEnumerable<HoroscopeDTO>> GetAll()
        {
            var day = DateTime.Now.Day.ToString();
            var cacheKey = $"{day}-{GetMonth()}";

            if (!_cache.TryGetValue<List<HoroscopeDTO>>(cacheKey, out List<HoroscopeDTO> resultList))
            {
                resultList = new List<HoroscopeDTO>();
                foreach (var horizon in _horizons)
                {
                    resultList.Add(await Get(horizon, GetMonth(), day));
                }

                _cache.Set(cacheKey, resultList);
            }

            return resultList;
        }

        public async Task<HoroscopeDTO> Get(string horoscope, string month, string day)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(BuildUrl(horoscope, month, day));
            var responseText = await response.Content.ReadAsStringAsync();

            var startIndex = responseText.IndexOf("\"articleBody\": \"") + 16;
            var endIndex = responseText.IndexOf("\"author\":");
            var returnText = responseText[startIndex..endIndex];
            returnText = returnText.Replace("\",", "");
            returnText = returnText.Replace("\n", "");
            returnText = returnText.Replace("\r", "");
            returnText = returnText.Replace("\u00A0", "");
            returnText = returnText.Trim();

            return new HoroscopeDTO
            {
                Text = returnText,
                Name = horoscope
            };
        }
        private string GetMonth() => _months[int.Parse(DateTime.Now.Month.ToString()) - 1];

        private string BuildUrl(string horoscope, string month, string day)
            => $"https://burc.co/{horoscope}-burcu/{day}-{month}-2020-yorumu";
    }
}
