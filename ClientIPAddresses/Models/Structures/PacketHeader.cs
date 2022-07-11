using System.Runtime.InteropServices;

namespace ClientIPAddresses.Models.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct PacketHeader
    {
        int Version;           // версия база данных
        fixed sbyte Name[32];          // название/префикс для базы данных
        ulong Timestamp;         // время создания базы данных
        public int Records;           // общее количество записей
        public uint OffsetRanges;     // смещение относительно начала файла до начала списка записей с геоинформацией
        public uint OffsetCities;     // смещение относительно начала файла до начала индекса с сортировкой по названию городов
        public uint OffsetLocations;  // смещение относительно начала файла до начала списка записей о местоположении       
    };
}
