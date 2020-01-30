using horoscope_crawler.Models;
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
        public HoroscopeService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HoroscopeDTO> GetDailyHoroscopeInterpretation(string horoscope, string month, string day)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(BuildUrl(horoscope, month, day));
            var responseText = await response.Content.ReadAsStringAsync();

            var startIndex = responseText.IndexOf("\"articleBody\": \"") + 16;
            var endIndex = responseText.IndexOf("\"author\":");
            var returnText = responseText.Substring(startIndex, endIndex - startIndex);
            returnText = returnText.Replace("\",", "");
            returnText = returnText.Replace("\n", "");
            returnText = returnText.Replace("\r", "");
            returnText = returnText.Replace("\u00A0", "");
            returnText = returnText.Trim();

            return new HoroscopeDTO { Text = returnText };
        }

        private string BuildUrl(string horoscope, string month, string day)
            => $"https://burc.co/{horoscope}-burcu/{day}-{month}-2020-yorumu";
    }
}
