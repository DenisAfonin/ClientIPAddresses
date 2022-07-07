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

        [HttpGet]
        [Route("ip/location")]
        public GEOInformation Get(string ip)
        {
            return _datFileReader.GetGEOInformationsByIP(ip);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}
