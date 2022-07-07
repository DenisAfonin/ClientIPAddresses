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
            long end = Stopwatch.GetTimestamp();
            var version = BitConverter.ToInt32(bytes, 0);
            var name = Encoding.Default.GetString(bytes, 4, 32);
            var timestamp = UnixTimeStampToDateTime((ulong)BitConverter.ToInt64(bytes, 36));
            var recordsAmount = BitConverter.ToInt32(bytes, 44);
            var offset_ranges = (uint)BitConverter.ToInt32(bytes, 48);
            var offset_cities = (uint)BitConverter.ToInt32(bytes, 52);
            OffsetLocations = (uint)BitConverter.ToInt32(bytes, 56);
            IPIntervalls = new IPIntervall[recordsAmount];
            for (var i = 0; i < recordsAmount; i++)
            {
                IPIntervalls[i] = new IPIntervall();
                IPIntervalls[i].IPFrom = (uint)BitConverter.ToInt32(bytes, (int)offset_ranges + i * 12);
                IPIntervalls[i].IPTo = (uint)BitConverter.ToInt32(bytes, (int)offset_ranges + 4 + i * 12);
                IPIntervalls[i].LocationIndex = (uint)BitConverter.ToInt32(bytes, (int)offset_ranges + 8 + i * 12);
            }
            Locations = new Location[recordsAmount];
            for (var i = 0; i < recordsAmount; i++)
            {
                var shiftIndex = i * 96;
                Locations[i] = new Location();
                Locations[i].AddressIndexInFile = shiftIndex;
                Locations[i].Country = Encoding.Default.GetString(bytes, (int)OffsetLocations + shiftIndex, 8);
                Locations[i].Region = Encoding.Default.GetString(bytes, (int)OffsetLocations + 8 + shiftIndex, 12);
                Locations[i].Postal = Encoding.Default.GetString(bytes, (int)OffsetLocations + 20 + shiftIndex, 12);
                Locations[i].City = Encoding.Default.GetString(bytes, (int)OffsetLocations + 32 + shiftIndex, 24);
                Locations[i].Organization = Encoding.Default.GetString(bytes, (int)OffsetLocations + 56 + shiftIndex, 32);
                Locations[i].Latitude = BitConverter.ToSingle(bytes, (int)OffsetLocations + 88 + shiftIndex);
                Locations[i].Longitude = BitConverter.ToSingle(bytes, (int)OffsetLocations + 92 + shiftIndex);
            }
            LocationIndexes = new int[recordsAmount];
            for (var i = 0; i < recordsAmount; i++)
            {
                LocationIndexes[i] = BitConverter.ToInt32(bytes, (int)(offset_cities + i * 4));
            }
            //long end = Stopwatch.GetTimestamp();
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
            var recordIndex = LocationIndexes[locationIndex];
            var location = Locations.FirstOrDefault(p => p.AddressIndexInFile == recordIndex);
            return new GEOInformation
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            };
        }

        private uint BinarySearchLocationIndexByIP(uint ip)
        {
            var arr = IPIntervalls;
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
            throw new Exception("IP Interval not found");
        }

        public Location[] GetLocationsByCity()
        {
            string[] myArr = Locations.GroupBy(p => p.City).Where(p => p.Count() > 1).Select(p => p.Key).ToArray();
            Array.Sort(myArr); //Apple Microsoft StackOverflow Yahoo 
            var index = Array.BinarySearch<string>(myArr, "cit_Gbqw4");
            return Locations;
        }

        private IPIntervall[] IPIntervalls { get; set; }
        private Location[] Locations { get; set; }
        private int[] LocationIndexes { get; set; }
        private uint OffsetLocations { get; set; }
    }
}
