using ClientIPAddresses.Interfaces;
using ClientIPAddresses.Models;
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
        public GEOInformation GetGEOInformationsByIP(string ip)
        {
            return _datFileReader.GetGEOInformationsByIP(ip);
        }

        [HttpGet("city/locations")]
        public List<Location> GetLocationsByCity(string city)
        {
            return _datFileReader.GetLocationsByCity(city);
        }
    }
}
