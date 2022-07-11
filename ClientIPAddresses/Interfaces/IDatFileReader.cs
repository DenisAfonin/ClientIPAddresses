using ClientIPAddresses.Models;

namespace ClientIPAddresses.Interfaces
{
    public interface IDatFileReader
    {
        GEOInformationDataContract? GetGEOInformationsByIP(string ipString);
        List<LocationDataContract>? GetLocationsByCity(string city);
    }
}
