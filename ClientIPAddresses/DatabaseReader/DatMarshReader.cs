using ClientIPAddresses.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClientIPAddresses.DatabaseReader
{
    public class DatMarshReader : IDatMarshReader
    {
        public unsafe DatMarshReader()
        {
            var start = Stopwatch.GetTimestamp();
            var bytes = File.ReadAllBytes(@"..\ClientIPAddresses\geobase.dat");

            fixed (byte* p = bytes)
            {
                IntPtr ptr = (IntPtr)p;
                PacketHeader obj = (PacketHeader)Marshal.PtrToStructure(ptr, typeof(PacketHeader));

                LocationH[] locs = new LocationH[obj.records];
                for (var i = 0; i < obj.records; i++)
                {
                    var shift = i * 12;
                    IntPtr newPtr = IntPtr.Add(ptr, (int)obj.offset_ranges + shift);
                    locs[i] = (LocationH)Marshal.PtrToStructure(newPtr, typeof(LocationH));
                }
                CoordH[] coords = new CoordH[obj.records];
                for (var i = 0; i < obj.records; i++)
                {
                    var shift = i * 96;
                    IntPtr newPtr = IntPtr.Add(ptr, (int)obj.offset_locations + shift);
                    coords[i] = (CoordH)Marshal.PtrToStructure(newPtr, typeof(CoordH));
                }
                int[] ind = new int[obj.records];
                for (var i = 0; i < obj.records; i++)
                {
                    var shift = i * 4;
                    IntPtr newPtr = IntPtr.Add(ptr, (int)obj.offset_cities + shift);
                    ind[i] = Marshal.ReadInt32(newPtr);
                }
                long end = Stopwatch.GetTimestamp();
                var timespan = end - start;
                var elapsedSpan = new TimeSpan(timespan);
                var ms = elapsedSpan.TotalMilliseconds;
            }
        }
    }
  
    struct PacketHeader
    {
        int version;           // версия база данных
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        string name;          // название/префикс для базы данных
        ulong timestamp;         // время создания базы данных
        public int records;           // общее количество записей
        public uint offset_ranges;     // смещение относительно начала файла до начала списка записей с геоинформацией
        public uint offset_cities;     // смещение относительно начала файла до начала индекса с сортировкой по названию городов
        public uint offset_locations;  // смещение относительно начала файла до начала списка записей о местоположении       
    };

    struct LocationH
    {
        uint ip_from;           // начало диапазона IP адресов
        uint ip_to;             // конец диапазона IP адресов
        uint location_index;    // индекс записи о местоположении
    }

    struct CoordH
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        string country;        // название страны (случайная строка с префиксом "cou_")
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        string region;        // название области (случайная строка с префиксом "reg_")
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        string postal;        // почтовый индекс (случайная строка с префиксом "pos_")
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        string city;          // название города (случайная строка с префиксом "cit_")
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        string organization;  // название организации (случайная строка с префиксом "org_")
        float latitude;          // широта
        float longitude;         // долгота
    }
}
