namespace ClientIPAddresses.Models
{
    public class LocationDataContract
    {
        public string Country { get; set; }        // название страны (случайная строка с префиксом "cou_")
        public string Region { get; set; }        // название области (случайная строка с префиксом "reg_")
        public string Postal { get; set; }        // почтовый индекс (случайная строка с префиксом "pos_")
        public string City { get; set; }          // название города (случайная строка с префиксом "cit_")
        public string Organization { get; set; }  // название организации (случайная строка с префиксом "org_")
        public float Latitude { get; set; }          // широта
        public float Longitude { get; set; }         // долгота
        public int AddressIndexInFile { get; set; }
    }
}
