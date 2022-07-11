using System.Text;

namespace ClientIPAddresses.Models.Structures
{
    unsafe struct Location
    {
        fixed sbyte Country[8];        // название страны (случайная строка с префиксом "cou_")
        fixed sbyte Region[12];        // название области (случайная строка с префиксом "reg_")
        fixed sbyte Postal[12];        // почтовый индекс (случайная строка с префиксом "pos_")
        fixed sbyte City[24];          // название города (случайная строка с префиксом "cit_")
        fixed sbyte Organization[32];  // название организации (случайная строка с префиксом "org_")
        public float Latitude;          // широта
        public float Longitude;         // долгота

        public string GetCountry()
        {
            var result = new StringBuilder(8);

            for (int i = 0; i < 8; i++)
            {
                result.Append((char)Country[i]);
            }

            return result.ToString().TrimEnd('\0');
        }

        public string GetRegion()
        {
            var result = new StringBuilder(12);

            for (int i = 0; i < 12; i++)
            {
                result.Append((char)Region[i]);
            }

            return result.ToString().TrimEnd('\0');
        }

        public string GetPostal()
        {
            var result = new StringBuilder(12);

            for (int i = 0; i < 12; i++)
            {
                result.Append((char)Postal[i]);
            }

            return result.ToString().TrimEnd('\0');
        }

        public string GetCity()
        {
            var result = new StringBuilder(24);

            for (int i = 0; i < 24; i++)
            {
                result.Append((char)City[i]);
            }

            return result.ToString().TrimEnd('\0');
        }

        public string GetOrg()
        {
            var result = new StringBuilder(32);

            for (int i = 0; i < 23; i++)
            {
                result.Append((char)Organization[i]);
            }

            return result.ToString();
        }
    }
}
