using System;
using System.Collections.Generic;
using System.Text;

namespace HashDictionary
{
    class Geolocation
    {
        
        float Latitude, Longitude;

        public Geolocation(float Lat, float Long)
        {
            Latitude = Lat;
            Longitude = Long;
        }

        public override bool Equals(object obj)
        {
            //overrides Equals and sets obj as geolocation.
            return Longitude.Equals((obj as Geolocation).Longitude) && Latitude.Equals((obj as Geolocation).Latitude);
        }
        public override int GetHashCode()
        {
            //10001 is the size of the hashtable
            return (360 * (int)Latitude + (int)Longitude) * 10001;
        }
    }
}
