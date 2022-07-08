﻿using ClientIPAddresses.Interfaces;
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
            //var start = Stopwatch.GetTimestamp();
            byte[] bytes;
            using (var fileStream = new FileStream(@"..\ClientIPAddresses\geobase.dat", FileMode.Open, FileAccess.Read))
            {
                long length = fileStream.Length;
                bytes = new byte[length];
                fileStream.Read(bytes, 0, bytes.Length);
            }
            //long end = Stopwatch.GetTimestamp();
            //var timespan = end - start;
            //var elapsedSpan = new TimeSpan(timespan);
            //var ms = elapsedSpan.TotalMilliseconds;
            var recordsAmount = BitConverter.ToInt32(bytes, 44);
            var offsetRanges = BitConverter.ToUInt32(bytes, 48);
            var offsetCities = BitConverter.ToUInt32(bytes, 52);
            var offsetLocations = BitConverter.ToUInt32(bytes, 56);
            iPIntervalls = new IPIntervall[recordsAmount];
            for (var i = 0; i < recordsAmount; i++)
            {
                var shiftIndex = i * 12;
                var shiftFromOffset = (int)offsetRanges + shiftIndex;
                iPIntervalls[i] = new IPIntervall
                {
                    IPFrom = BitConverter.ToUInt32(bytes, shiftFromOffset),
                    IPTo = BitConverter.ToUInt32(bytes, shiftFromOffset + 4),
                    LocationIndex = BitConverter.ToUInt32(bytes, shiftFromOffset + 8)
                };
            }
            locations = new Location[recordsAmount];

            for (var i = 0; i < recordsAmount; i++)
            {
                var shiftIndex = i * 96;
                var shiftFromOffset = (int)offsetLocations + shiftIndex;
                var location = locations[i] = new Location
                {
                    AddressIndexInFile = shiftIndex,
                    Country = Encoding.Default.GetString(bytes, shiftFromOffset, 8).TrimEnd('\0'),
                    Region = Encoding.Default.GetString(bytes, shiftFromOffset + 8, 12).TrimEnd('\0'),
                    Postal = Encoding.Default.GetString(bytes, shiftFromOffset + 20, 12).TrimEnd('\0'),
                    City = Encoding.Default.GetString(bytes, shiftFromOffset + 32, 24).TrimEnd('\0'),
                    Organization = Encoding.Default.GetString(bytes, shiftFromOffset + 56, 32).TrimEnd('\0'),
                    Latitude = BitConverter.ToSingle(bytes, shiftFromOffset + 88),
                    Longitude = BitConverter.ToSingle(bytes, shiftFromOffset + 92)
                };
                if (locationsDictionary.ContainsKey(location.City))
                {
                    locationsDictionary[location.City].Add(location);
                }
                else
                {
                    locationsDictionary.Add(location.City, new List<Location> { location });
                }
            }

            locationIndexes = new int[recordsAmount];
            for (var i = 0; i < recordsAmount; i++)
            {
                locationIndexes[i] = BitConverter.ToInt32(bytes, (int)(offsetCities + i * 4));
            }
        }

        public GEOInformation? GetGEOInformationsByIP(string ipString)
        {
            bool isIPValid = IPAddress.TryParse(ipString, out IPAddress ipAddress);
            if (!isIPValid)
            {
                return null;
            }
            var ipBytes = ipAddress.GetAddressBytes();
            var ip = (uint)ipBytes[0] << 24;
            ip += (uint)ipBytes[1] << 16;
            ip += (uint)ipBytes[2] << 8;
            ip += (uint)ipBytes[3];
            var locationIndex = BinarySearchLocationIndexByIP(ip);
            if (!locationIndex.HasValue || locationIndexes.Length - 1 < locationIndex)
            {
                return null;
            }
            var recordIndex = locationIndexes[locationIndex.Value];
            var location = locations.FirstOrDefault(p => p.AddressIndexInFile == recordIndex);
            if (location == null)
            {
                return null;
            }
            return new GEOInformation
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            };
        }

        private uint? BinarySearchLocationIndexByIP(uint ip)
        {
            int minNum = 0;
            int maxNum = iPIntervalls.Length - 1;

            while (minNum <= maxNum)
            {
                int mid = (minNum + maxNum) / 2;
                if (ip < iPIntervalls[mid].IPTo && ip > iPIntervalls[mid].IPFrom)
                {
                    return iPIntervalls[mid].LocationIndex;
                }
                else if (ip < iPIntervalls[mid].IPFrom)
                {
                    maxNum = mid - 1;
                }
                else
                {
                    minNum = mid + 1;
                }
            }
            return null;
        }

        public List<Location>? GetLocationsByCity(string city)
        {
            if (!locationsDictionary.ContainsKey(city))
            {
                return null;
            }
            return locationsDictionary[city];
        }

        private IPIntervall[] iPIntervalls;
        private Location[] locations;
        private int[] locationIndexes;
        private Dictionary<string, List<Location>> locationsDictionary = new Dictionary<string, List<Location>>();
    }
}
