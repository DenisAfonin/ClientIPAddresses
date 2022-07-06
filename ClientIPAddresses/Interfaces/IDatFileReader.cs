using ClientIPAddresses.Models;

namespace ClientIPAddresses.Interfaces
{
    public interface IDatFileReader
    {
        IPIntervall[] IPIntervalls { get; }
        Location[] Locations { get; }
        int[] LocationIndexes { get; }
    }
}
