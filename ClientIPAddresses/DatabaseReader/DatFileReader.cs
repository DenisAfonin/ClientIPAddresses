using ClientIPAddresses.Interfaces;
using ClientIPAddresses.Models;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

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
            using (BinaryReader br = new BinaryReader(fileStream, Encoding.UTF8, false))
            {
                long length = fileStream.Length;
                bytes = new byte[length];
                br.Read(bytes, 0, bytes.Length);
            }

            var version = BitConverter.ToInt32(bytes, 0);
            var name = Encoding.Default.GetString(bytes, 4, 32);
            var timestamp = UnixTimeStampToDateTime((ulong)BitConverter.ToInt64(bytes, 36));
            var recordsAmount = BitConverter.ToInt32(bytes, 44);
            var offset_ranges = (uint)BitConverter.ToInt32(bytes, 48);
            var offset_cities = (uint)BitConverter.ToInt32(bytes, 52);
            var offset_locations = (uint)BitConverter.ToInt32(bytes, 56);
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
                Locations[i] = new Location();
                Locations[i].Country = Encoding.Default.GetString(bytes, (int)offset_locations + i * 96, 8);
                Locations[i].Region = Encoding.Default.GetString(bytes, (int)offset_locations + 8 + i * 96, 12);
                Locations[i].Postal = Encoding.Default.GetString(bytes, (int)offset_locations + 20 + i * 96, 12);
                Locations[i].City = Encoding.Default.GetString(bytes, (int)offset_locations + 32 + i * 96, 24);
                Locations[i].Organization = Encoding.Default.GetString(bytes, (int)offset_locations + 56 + i * 96, 32);
                Locations[i].Latitude = BitConverter.ToSingle(bytes, (int)offset_locations + 88 + i * 96);
                Locations[i].Longitude = BitConverter.ToSingle(bytes, (int)offset_locations + 92 + i * 96);
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

        public IPIntervall[] IPIntervalls { get; private set; }
        public Location[] Locations { get; private set; }
        public int[] LocationIndexes { get; private set; }
    }
}
