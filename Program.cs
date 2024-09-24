using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HashtableTester;
using System.IO;
using System.Globalization;

namespace HashDictionary
{
    class Program
    {
        public HashDictionary<Geolocation, City> keyValuePairs = new HashDictionary<Geolocation, City>();
        
        static void Main(string[] args)
        {
            //parsing string to become floats, putting them into the float variables below.
            //we can get values from the tryGetValue and put them into a City object, and 
            //from that object, we can print the result. 
            Program program = new Program();
            bool Test = false;
            City city;
            string LatitudeString, LongitudeString;
            float Latitude, Longitude;
            if (Test)
            HashtableTester.TestDriver.Instance.Run(new HashTable<int, int>(), 10000);
            else
            {
                program.LoadCities();
                Console.WriteLine("Enter Longitude: ");
                LatitudeString = Console.ReadLine();
                Console.WriteLine("Enter Latitude: ");
                LongitudeString = Console.ReadLine();
                float.TryParse(LatitudeString, NumberStyles.Float, CultureInfo.InvariantCulture, out Latitude);
                float.TryParse(LongitudeString, NumberStyles.Float, CultureInfo.InvariantCulture, out Longitude);
                if(program.keyValuePairs.TryGetValue(new Geolocation(Latitude, Longitude), out city))
                {
                    Console.WriteLine($" City: {city.CityName}\nPopulation: {city.Population}");
                    return;
                }

                else
                    Console.WriteLine("No city here!");
            }
        }

        public void LoadCities()
        {

            foreach (string line in File.ReadLines("..\\cities100000.txt"))
            {
                string[] data = line.Split(new char[] {'\t'});

                keyValuePairs.Add(new Geolocation(float.Parse(data[1], CultureInfo.InvariantCulture), float.Parse(data[2], CultureInfo.InvariantCulture)), new City(data[0], data[3])); // Load data from all the cities from the file
            }
        }
    }
}
