using System;
using SAAB_Maritime.Controller;
using SAAB_Maritime.Model;

namespace SAAB_Maritime
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Ship Updater Program!");

            // Set latitude and longitude to cover the entire world
            double minLat = -90.0;
            double minLong = -180.0;
            double maxLat = 90.0;
            double maxLong = 180.0;
            private string DatabasePath = "..\\..\\Resources\\2024-04-12_00_00_00-23_59_59_AIS_DATA.db"; // change as such where is you data

            // Set start and end dates
            DateTime startDate = new DateTime(2022, 1, 1); // Start date in 2022
            DateTime endDate = DateTime.Now; // End date is now

            // Set a ship limit (you can modify this as needed)
            int shipLimit = int.MaxValue; // Example limit

            // Create an instance of Updater
            Updater updater = new Updater(minLat, minLong, maxLat, maxLong, startDate, endDate, shipLimit, databasePath);
            Vessel? v = updater.Getship(209223000);

            //update the position of v until it finished
            while(!v.GetArrival)
            {
                if (v != null)
                {
                    Console.WriteLine("MMSI: " + v.MMSI() + " Lat: " + v.CurrentPos().GetLatitude() + " Lng: " + v.CurrentPos().GetLongitude() + " Ship Data ended: " + v.GetArrival);
                    updater.updatePosition(v);
                }
            }
            Console.WriteLine("Finished displaying all ships.");
        }
    }
}
