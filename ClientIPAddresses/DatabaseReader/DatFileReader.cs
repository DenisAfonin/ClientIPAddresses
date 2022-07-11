using ClientIPAddresses.Interfaces;
using ClientIPAddresses.Models;
using ClientIPAddresses.Models.Structures;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace ClientIPAddresses.DatabaseReader
{
    public class DatFileReader : IDatFileReader
    {
        private IPIntervall[] iPIntervalls;
        private Location[] locations;
        private int[] locationIndexes;
        public unsafe DatFileReader()
        {
            var bytes = File.ReadAllBytes(@"..\ClientIPAddresses\geobase.dat");

            fixed (byte* p = bytes)
            {
                IntPtr ptr = (IntPtr)p;
                PacketHeader obj = (PacketHeader)Marshal.PtrToStructure(ptr, typeof(PacketHeader));

                iPIntervalls = new IPIntervall[obj.Records];
                locations = new Location[obj.Records];
                locationIndexes = new int[obj.Records];
                for (var i = 0; i < obj.Records; i++)
                {
                    IntPtr newLocPtr = IntPtr.Add(ptr, (int)obj.OffsetRanges + i * 12);
                    iPIntervalls[i] = (IPIntervall)Marshal.PtrToStructure(newLocPtr, typeof(IPIntervall));

                    IntPtr newCorPtr = IntPtr.Add(ptr, (int)obj.OffsetLocations + i * 96);
                    locations[i] = (Location)Marshal.PtrToStructure(newCorPtr, typeof(Location));

                    IntPtr newPtr = IntPtr.Add(ptr, (int)obj.OffsetCities + i * 4);
                    locationIndexes[i] = Marshal.ReadInt32(newPtr);
                }
            }
        }

        public GEOInformationDataContract? GetGEOInformationsByIP(string ipString)
        {
            bool isIPValid = IPAddress.TryParse(ipString, out IPAddress ipAddress);
            if (!isIPValid)
            {
                return null;
            }
            var ipBytes = ipAddress.GetAddressBytes();
            var ip = (uint)ipBytes[0] << 24;
            ip += (uint)ipBytes[1] << 16;
            ip += (uint)ipBytes[2] << 8;
            ip += (uint)ipBytes[3];
            var locationIndex = BinarySearchLocationIndexByIP(ip);
            if (!locationIndex.HasValue || locationIndexes.Length - 1 < locationIndex)
            {
                return null;
            }
            var recordIndex = locationIndexes[locationIndex.Value];
            var location = locations[recordIndex / 96];
            return new GEOInformationDataContract
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            };
        }

        private uint? BinarySearchLocationIndexByIP(uint ip)
        {
            int minNum = 0;
            int maxNum = iPIntervalls.Length - 1;

            while (minNum <= maxNum)
            {
                int mid = (minNum + maxNum) / 2;
                if (ip < iPIntervalls[mid].IPTo && ip > iPIntervalls[mid].IPFrom)
                {
                    return iPIntervalls[mid].LocationIndex;
                }
                else if (ip < iPIntervalls[mid].IPFrom)
                {
                    maxNum = mid - 1;
                }
                else
                {
                    minNum = mid + 1;
                }
            }
            return null;
        }

        public List<LocationDataContract> GetLocationsByCity(string city)
        {
            return locations.Where(p => p.GetCity() == city).Select(p => new LocationDataContract
            {
                City = city,
                Country = p.GetCountry(),
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Organization = p.GetOrg(),
                Postal = p.GetPostal(),
                Region = p.GetRegion(),
            }).ToList();
        }
    }
}
