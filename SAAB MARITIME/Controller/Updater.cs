
using SAAB_Maritime.Model;

namespace SAAB_Maritime.Controller
{
    internal class Updater
    {
        private List<Vessel> ships;
        private ShipController shipController;
        //private string DatabasePath = "..\\..\\Resources\\2024-04-12_00_00_00-23_59_59_AIS_DATA.db";

        private List<Vessel> shipListToRemove = new List<Vessel>();

        public Updater(double minlat, double minlong, double maxlat, double maxlong, DateTime startDate, DateTime endDate, int shipLimit, string DatabasePath)
        {
            shipController = new ShipController(minlat, minlong, maxlat, maxlong, startDate, endDate, shipLimit, DatabasePath);
            if (shipController.generateShips())
            {
                ships = shipController.getShips();
            }
            else
            {
                throw new Exception("Ship couldn't be generated");
            }
        }

        public Vessel? Getship(int MMSI)
        {
            foreach (Vessel v in ships)
            {
                if(v.MMSI() == MMSI)
                {
                    if (v.needPos())
                    {
                        shipController.updateSingleShip(v);
                        v.noNeed();
                    }
                    return v;
                }
            }

            return null;
        }

        public void updatePosition(Vessel v)
        {
            if (v.needPos())
            {
                shipController.updateSingleShip(v);
                v.noNeed();
            }
            v.Update();
        }

    }
}
