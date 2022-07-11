namespace ClientIPAddresses.Models
{
    public class IPIntervallDataContract
    {
        public uint IPFrom { get; set; }       // начало диапазона IP адресов
        public uint IPTo { get; set; }            // конец диапазона IP адресов
        public uint LocationIndex { get; set; }   // индекс записи о местоположении
    }
}
