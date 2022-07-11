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
                    coords[i] = (CoordH)Marshal.PtrToStructure(newPtr, typeof(CoordH));
                }
                long end = Stopwatch.GetTimestamp();
                var timespan = end - start;
                var elapsedSpan = new TimeSpan(timespan);
                var ms = elapsedSpan.TotalMilliseconds;
            }
           

        }

        //private Object ByteArrayToObject(byte[] arrBytes)
        //{
        //    MemoryStream memStream = new MemoryStream();
        //    BinaryFormatter binForm = new BinaryFormatter();
        //    memStream.Write(arrBytes, 0, arrBytes.Length);
        //    memStream.Seek(0, SeekOrigin.Begin);
        //    Object obj = (Object)binForm.Deserialize(memStream);
        //    return obj;
        //}

        //unsafe void Load(byte[] buffer, int offset, int length)
        //{
        //    fixed (byte* ptr = buffer)
        //    {
        //        PacketHeader* data = (PacketHeader*)(ptr + 0);
        //        var results = new IPIntervall[60 / sizeof(PacketHeader)];
        //        var result = new IPIntervall();
        //        //for (int i = 0; i < results.Length; i++)
        //        //{
        //        //    results[i] = new IPIntervall();
        //        //    data++;
        //        //}
        //        var a = result;
        //    }
        //}

        private unsafe object BytesToDataStruct(byte[] bytes, Type type)
        {

            //int size = Marshal.SizeOf(type);

            //if (size > bytes.Length)
            //{
            //    return null;
            //}

            //IntPtr unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
            //Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);
            //// Call unmanaged code
            //Marshal.FreeHGlobal(unmanagedPointer);

            //IntPtr structPtr = Marshal.AllocHGlobal(size);
            //Marshal.Copy(bytes, 0, structPtr, size);
            //object obj = Marshal.PtrToStructure(structPtr, type);
            //Marshal.FreeHGlobal(structPtr);
            //return obj;

            return null;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct PacketHeader
    {
        int version;           // версия база данных
        fixed sbyte name[32];          // название/префикс для базы данных
        ulong timestamp;         // время создания базы данных
        public int records;           // общее количество записей
        public uint offset_ranges;     // смещение относительно начала файла до начала списка записей с геоинформацией
        public uint offset_cities;     // смещение относительно начала файла до начала индекса с сортировкой по названию городов
        public uint offset_locations;  // смещение относительно начала файла до начала списка записей о местоположении       
    };

    unsafe struct LocationH
    {
        uint ip_from;           // начало диапазона IP адресов
        uint ip_to;             // конец диапазона IP адресов
        uint location_index;    // индекс записи о местоположении
    }

    unsafe struct CoordH
    {
        fixed sbyte country[8];        // название страны (случайная строка с префиксом "cou_")
        fixed sbyte region[12];        // название области (случайная строка с префиксом "reg_")
        fixed sbyte postal[12];        // почтовый индекс (случайная строка с префиксом "pos_")
        fixed sbyte city[24];          // название города (случайная строка с префиксом "cit_")
        fixed sbyte organization[32];  // название организации (случайная строка с префиксом "org_")
        float latitude;          // широта
        float longitude;         // долгота
    }
}
