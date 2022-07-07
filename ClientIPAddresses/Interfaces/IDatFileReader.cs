using ClientIPAddresses.Models;

namespace ClientIPAddresses.Interfaces
{
    public interface IDatFileReader
    {
        GEOInformation GetGEOInformationsByIP(string ipString);
        List<Location> GetLocationsByCity(string city);
    }
}
