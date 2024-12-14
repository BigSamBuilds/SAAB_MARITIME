using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SAAB_Maritime.Model;

namespace SAAB_Maritime.Controller
{
    internal class ShipController
    {
        private List<Vessel> _ships;
        private int _shipAmount;
        private DatabaseQuery _db;
        private double _minlat;
        private double _maxlat;
        private double _minlon;
        private double _maxlon;
        private DateTime _startDate;
        private DateTime _endDate;
        private int shipPositionObjectLimit = 200; // this is the limitation of postion list inside the Ship object 

        public ShipController(double minlat, double minlong, double maxlat, double maxlong, DateTime startDate, DateTime endDate, int shipLimit, string DatabasePath)
        {
            _ships = new List<Vessel>();
            _db = new DatabaseQuery(new DatabaseHandler($"Data Source={DatabasePath};"));
            _minlat = minlat;
            _maxlat = maxlat;
            _minlon = minlong;
            _maxlon = maxlong;
            _startDate = startDate;
            _endDate = endDate;
            this._shipAmount = shipLimit;
        }

        public bool generateShips()
        {
            Dictionary<string, List<object>> info = _db.GetAllFromShips(_maxlon, _minlon, _maxlat, _minlat, _startDate.ToString("yyyy-MM-dd HH:mm:ss"), _endDate.ToString("yyyy-MM-dd HH:mm:ss"), _shipAmount);
            if (info != null && info.Count > 0 && info["MMSI"] != null)
            {
                try
                {
                    for (int i = 0; i < info["MMSI"].Count; i++)
                    {
                        //hämtar ett specifik skepps positioner inom ett visst intervall, 2 minuter atm
                        Dictionary<string, List<object>> posTable = _db.GetVessel(int.Parse(info["MMSI"][i].ToString()), _maxlon, _minlon, _maxlat, _minlat, _startDate.ToString("yyyy-MM-dd HH:mm:ss"), _endDate.ToString("yyyy-MM-dd HH:mm:ss"), shipPositionObjectLimit);
                        List<Position> positions = getPositions(posTable);

                        DateTime.TryParseExact(posTable["ShipRealTimeDate"][0].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime start);

                        _ships.Add(new Ship(
                            int.Parse(info["ID"][i].ToString()),
                            int.Parse(info["MMSI"][i].ToString()),
                            posTable["radio_Callsign"][0].ToString(),
                            positions,
                            info["namn"][i].ToString(),
                            info["modell"][i].ToString(),
                            start,
                            posTable["ETA"][0].ToString(),
                            "unknown", // feature
                            "Unknown" // feature
                            ));

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<Position> getPositions(Dictionary<string, List<object>> posTable)
        {
            List<Position> positions = new List<Position>();

            for (int i = 0; i < posTable["MMSI"].Count; i++)
            {
                DateTime.TryParseExact(posTable["StartDate"][i].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt);

                Position p = new Position(
                    double.Parse(posTable["longitud"][i].ToString()),
                    double.Parse(posTable["latitud"][i].ToString()),
                    dt,
                    double.Parse(posTable["course"][i].ToString()),
                    double.Parse(posTable["speed"][i].ToString())
                );

                positions.Add(p);
            }

            return positions; // Return the new list
        }


        public List<Vessel> getShips() {
            _ships.Sort();
            return _ships;
        }

        public void updateSingleShip(Vessel v)
        {
            //hämtar ett specifik skepps positioner inom ett visst intervall, 2 minuter atm
            Dictionary<string, List<object>> posTable = _db.GetVessel(v.MMSI(), _maxlon, _minlon, _maxlat, _minlat, v.CurrentPos().GetDateTime().ToString("yyyy-MM-dd HH:mm:ss"), _endDate.ToString("yyyy-MM-dd HH:mm:ss"), shipPositionObjectLimit);
            List<Position> positions = getPositions(posTable);
            if (positions.Count <= 1)
            {
                ((Ship)v).GetArrival = true; // tag the ship ass arrived 
            }
            v.setNextPositions(positions); 
        }
    }
}
