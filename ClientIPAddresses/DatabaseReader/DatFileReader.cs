using ClientIPAddresses.Interfaces;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace ClientIPAddresses.DatabaseReader
{
    public class DatFileReader : IDatFileReader
    {
        public object Read()
        {
            using (FileStream fileStream = new FileStream(@"..\ClientIPAddresses\geobase.dat", FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fileStream, Encoding.UTF8, false))
            {
                //long length = fileStream.Length;
                //byte[] bytes = new byte[length];
                ////Read the contents of the file and save it to the byte array
                //br.Read(bytes, 0, bytes.Length);
                ////Convert byte array to string
                //string str = Encoding.Default.GetString(bytes);
                //  var c = br.ReadBytes(60);
                var version = br.ReadInt32();
                var nameBytes = new byte[32];
                for (int i = 0; i < nameBytes.Length; i++)
                {
                    nameBytes[i] = br.ReadByte();
                }
                var name = Encoding.Default.GetString(nameBytes, 0, nameBytes.Length);
                DateTime creationDate = GetDTCTime((ulong)br.ReadInt64());
                var recordsAmount = br.ReadInt32();
                var offset_ranges = (uint)br.ReadInt32();
                var offset_cities = (uint)br.ReadInt32();
                var offset_locations = (uint)br.ReadInt32();
                var ipIntervals = new List<IPIntervall>();
                for (var i = 0; i < recordsAmount; i++)
                {
                    var ipInterval = new IPIntervall();
                    ipInterval.ip_from = (uint)br.ReadInt32();
                    ipInterval.ip_to = (uint)br.ReadInt32();
                    ipInterval.location_index = (uint)br.ReadInt32();
                    ipIntervals.Add(ipInterval);
                }
                var locations = new List<Location>();
                for (var i = 0; i < recordsAmount; i++)
                {
                    var location = new Location();
                    location.country = byteArrayToString(br, 8);
                    location.region = byteArrayToString(br, 12);
                    location.postal = byteArrayToString(br, 12);
                    location.city = byteArrayToString(br, 24);
                    location.organization = byteArrayToString(br, 32);
                    location.latitude = br.ReadSingle();
                    location.longitude = br.ReadSingle();
                    locations.Add(location);
                }
                var recordIndexes = new List<int>();
                for (var i = 0; i < recordsAmount; i++)
                {
                    recordIndexes.Add(br.ReadInt32());
                }
                return 0;
            }
        }

        private string byteArrayToString(BinaryReader br, short numberOfByte)
        {
            var byteArray = new byte[numberOfByte];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = br.ReadByte();
            }
            var res = Encoding.Default.GetString(byteArray, 0, byteArray.Length);
            return res;
        }

        DateTime GetDTCTime(ulong nanoseconds, ulong ticksPerNanosecond)
        {
            DateTime pointOfReference = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long ticks = (long)(nanoseconds / ticksPerNanosecond);
            return pointOfReference.AddTicks(ticks);
        }

        DateTime GetDTCTime(ulong nanoseconds)
        {
            return GetDTCTime(nanoseconds, 100);
        }

        public class IPIntervall
        {
            public uint ip_from;           // начало диапазона IP адресов
            public uint ip_to;             // конец диапазона IP адресов
            public uint location_index;    // индекс записи о местоположении
        }

        public class Location
        {
            public string country;        // название страны (случайная строка с префиксом "cou_")
            public string region;        // название области (случайная строка с префиксом "reg_")
            public string postal;        // почтовый индекс (случайная строка с префиксом "pos_")
            public string city;          // название города (случайная строка с префиксом "cit_")
            public string organization;  // название организации (случайная строка с префиксом "org_")
            public float latitude;          // широта
            public float longitude;         // долгота
        }
    }
}
