using ClientIPAddresses.Interfaces;
using ClientIPAddresses.Models;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace ClientIPAddresses.DatabaseReader
{
    public class DatFileReader : IDatFileReader
    {
        public DatFileReader()
        {
            Read();
        }

        private void Read()
        {
            long start = Stopwatch.GetTimestamp();
            byte[] bytes;
            using (FileStream fileStream = new FileStream(@"..\ClientIPAddresses\geobase.dat", FileMode.Open, FileAccess.Read))
            {
                long length = fileStream.Length;
                bytes = new byte[length];
                fileStream.Read(bytes, 0, bytes.Length);
            }
            //long end = Stopwatch.GetTimestamp();
            var version = BitConverter.ToInt32(bytes, 0);
            var name = Encoding.Default.GetString(bytes, 4, 32).TrimEnd('\0');
            var timestamp = UnixTimeStampToDateTime((ulong)BitConverter.ToInt64(bytes, 36));
            var recordsAmount = BitConverter.ToInt32(bytes, 44);
            var offset_ranges = (uint)BitConverter.ToInt32(bytes, 48);
            var offset_cities = (uint)BitConverter.ToInt32(bytes, 52);
            OffsetLocations = (uint)BitConverter.ToInt32(bytes, 56);
            IPIntervalls = new IPIntervall[recordsAmount];
            for (var i = 0; i < recordsAmount; i++)
            {
                var shiftIndex = i * 12;
                var shiftFromOffset = (int)offset_ranges + shiftIndex;
                var iPIntervall = IPIntervalls[i] = new IPIntervall();
                iPIntervall.IPFrom = (uint)BitConverter.ToInt32(bytes, shiftFromOffset);
                iPIntervall.IPTo = (uint)BitConverter.ToInt32(bytes, shiftFromOffset + 4);
                iPIntervall.LocationIndex = (uint)BitConverter.ToInt32(bytes, shiftFromOffset + 8);
            }
            Locations = new Location[recordsAmount];
            LocationsDictionary = new Dictionary<string, List<Location>>();
            for (var i = 0; i < recordsAmount; i++)
            {
                var shiftIndex = i * 96;
                var shiftFromOffset = (int)OffsetLocations + shiftIndex;
                var location = Locations[i] = new Location();
                location.AddressIndexInFile = shiftIndex;
                location.Country = Encoding.Default.GetString(bytes, shiftFromOffset, 8).TrimEnd('\0');
                location.Region = Encoding.Default.GetString(bytes, shiftFromOffset + 8, 12).TrimEnd('\0');
                location.Postal = Encoding.Default.GetString(bytes, shiftFromOffset + 20, 12).TrimEnd('\0');
                location.City = Encoding.Default.GetString(bytes, shiftFromOffset + 32, 24).TrimEnd('\0');
                location.Organization = Encoding.Default.GetString(bytes, shiftFromOffset + 56, 32).TrimEnd('\0');
                location.Latitude = BitConverter.ToSingle(bytes, shiftFromOffset + 88);
                location.Longitude = BitConverter.ToSingle(bytes, shiftFromOffset + 92);
                if (LocationsDictionary.ContainsKey(location.City))
                {
                    LocationsDictionary[location.City].Add(location);
                }
                else
                {
                    LocationsDictionary.Add(location.City, new List<Location> { location });
                }
            }

            LocationIndexes = new int[recordsAmount];
            for (var i = 0; i < recordsAmount; i++)
            {
                LocationIndexes[i] = BitConverter.ToInt32(bytes, (int)(offset_cities + i * 4));
            }
            long end = Stopwatch.GetTimestamp();
            var timespan = end - start;
            TimeSpan elapsedSpan = new TimeSpan(timespan);
            var ms = elapsedSpan.Milliseconds;
        }

        private DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        public GEOInformation GetGEOInformationsByIP(string ipString)
        {
            var ipAddress = IPAddress.Parse(ipString);
            var ipBytes = ipAddress.GetAddressBytes();
            var ip = (uint)ipBytes[0] << 24;
            ip += (uint)ipBytes[1] << 16;
            ip += (uint)ipBytes[2] << 8;
            ip += (uint)ipBytes[3];
            var locationIndex = BinarySearchLocationIndexByIP(ip);
            if (LocationIndexes.Length - 1 < locationIndex)
            {
                throw new KeyNotFoundException("Location index not found");
            }
            var recordIndex = LocationIndexes[locationIndex];
            var location = Locations.FirstOrDefault(p => p.AddressIndexInFile == recordIndex);
            if (location == null)
            {
                throw new KeyNotFoundException("Location not found");
            }
            return new GEOInformation
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            };
        }

        private uint BinarySearchLocationIndexByIP(uint ip)
        {
            int minNum = 0;
            int maxNum = IPIntervalls.Length - 1;

            while (minNum <= maxNum)
            {
                int mid = (minNum + maxNum) / 2;
                if (ip < IPIntervalls[mid].IPTo && ip > IPIntervalls[mid].IPFrom)
                {
                    return IPIntervalls[mid].LocationIndex;
                }
                else if (ip < IPIntervalls[mid].IPFrom)
                {
                    maxNum = mid - 1;
                }
                else
                {
                    minNum = mid + 1;
                }
            }
            throw new KeyNotFoundException("IP Interval not found");
        }

        public List<Location> GetLocationsByCity(string city)
        {
            if (!LocationsDictionary.ContainsKey(city))
            {
                throw new KeyNotFoundException("City not found");
            }
            return LocationsDictionary[city];
        }

        private IPIntervall[] IPIntervalls { get; set; }
        private Location[] Locations { get; set; }
        private int[] LocationIndexes { get; set; }
        private uint OffsetLocations { get; set; }
        private Dictionary<string, List<Location>> LocationsDictionary { get; set; }
    }
}
