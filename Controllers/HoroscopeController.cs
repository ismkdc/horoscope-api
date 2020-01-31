using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using horoscope_crawler.Models;
using horoscope_crawler.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace horoscope_crawler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HoroscopeController : ControllerBase
    {
        private readonly HoroscopeService _horoscopeService;

        public HoroscopeController(HoroscopeService horoscopeService)
        {
            _horoscopeService = horoscopeService;
        }

        [HttpGet]
        public async Task<IEnumerable<HoroscopeDTO>> Get() =>
            await _horoscopeService.GetAll();
    }
}
