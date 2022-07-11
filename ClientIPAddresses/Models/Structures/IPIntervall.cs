namespace ClientIPAddresses.Models.Structures
{
    unsafe struct IPIntervall
    {
        public uint IPFrom;           // начало диапазона IP адресов
        public uint IPTo;             // конец диапазона IP адресов
        public uint LocationIndex;    // индекс записи о местоположении
    }
}
