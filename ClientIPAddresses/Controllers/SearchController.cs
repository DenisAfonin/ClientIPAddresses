using ClientIPAddresses.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientIPAddresses.Controllers
{
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IDatFileReader _datFileReader;
        public SearchController(IDatFileReader datFileReader)
        {
            _datFileReader = datFileReader;
        }

        [HttpGet("ip/location")]
        public IActionResult GetGEOInformationsByIP(string ip)
        {
            var result = _datFileReader.GetGEOInformationsByIP(ip);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("city/locations")]
        public IActionResult GetLocationsByCity(string city)
        {
            var result = _datFileReader.GetLocationsByCity(city);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
