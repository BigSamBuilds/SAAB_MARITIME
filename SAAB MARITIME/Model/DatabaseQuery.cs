using System.Data.SQLite;

namespace SAAB_Maritime.Model
{
    // TO - DO 
    //1. Hämta allting från SHIPS (diskriminera baserad på position) DONE
    //2. Hämta alla fartyg (inom ett viss område, minst den här tiden), returnera 


    internal class DatabaseQuery
    {
        private readonly DatabaseHandler databaseHandler;

        public DatabaseQuery(DatabaseHandler handler)
        {
            databaseHandler = handler;
        }

        //TO DO
        //GetVessel tar in en MMSI int, inom arean, efter datumet. Max antal positioner (200). Returnerar MMSI, Position. NOT *

        public Dictionary<string, List<object>> GetVessel(int MMSI, double maxLng, double minLng, double maxLat, double minLat, string date, string dateLimit, int limit)
        {
            //Skapa Queryn Här istället.


            var result = new Dictionary<string, List<object>>();

            try
            {
                databaseHandler.dbOpen();



                string query = "SELECT * FROM Positions WHERE MMSI IS @MMSI AND longitud BETWEEN @minLng AND @maxLng and latitud BETWEEN @minLat AND @maxLat AND StartDate >= @date AND StartDate <= @dateLimit LIMIT @shiplimit";

                using (SQLiteCommand command = new SQLiteCommand(query, (SQLiteConnection)databaseHandler.GetConnector()))
                {



                   command.Parameters.AddWithValue("@MMSI", MMSI);
                   command.Parameters.AddWithValue("@minLat", minLat);
                   command.Parameters.AddWithValue("@maxLat", maxLat);
                   command.Parameters.AddWithValue("@minLng", minLng);
                   command.Parameters.AddWithValue("@maxLng", maxLng);
                   command.Parameters.AddWithValue("@date", date);
                   command.Parameters.AddWithValue("@dateLimit", dateLimit);
                   command.Parameters.AddWithValue("@shiplimit", limit);



                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {



                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                if (!result.ContainsKey(columnName))
                                {
                                    result[columnName] = new List<object>();
                                }
                                result[columnName].Add(reader.GetValue(i));
                            }
                        }
                    }
                }
            }
            finally
            {
                databaseHandler.dbClose();
            }
            return result;
        }


        //MBY REMOVE

        public Dictionary<string, List<object>> GetAllDate(double maxLng, double minLng, double maxLat, double minLat, string date)
        {

            var result = new Dictionary<string, List<object>>();

            try
            {
                databaseHandler.dbOpen();
                // longitud
                // latitud

                //2024-04-10 11:00:11   

                string query = "SELECT * FROM Ships";

                using (SQLiteCommand command = new SQLiteCommand(query, (SQLiteConnection)databaseHandler.GetConnector()))
                {
                    command.Parameters.AddWithValue("@minLng", minLat);
                    command.Parameters.AddWithValue("@maxLat", maxLat);
                    command.Parameters.AddWithValue("@minLng", minLng);
                    command.Parameters.AddWithValue("@maxLng", maxLng);
                    command.Parameters.AddWithValue("date", date);


                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) {
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    if (!result.ContainsKey(columnName))
                                    {
                                        result[columnName] = new List<object>();
                                    }
                                    result[columnName].Add(reader.GetValue(i));
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                databaseHandler.dbClose();
            }
            //Console.WriteLine(result["MMSI"].Count + "          GetAllDate");
            return result;
        }




        //Get all from ships, based on position and from a certain date. MAX 2000

        public Dictionary<string, List<object>> GetAllFromShips(double maxLng, double minLng, double maxLat, double minLat, string date, string dateLimit, int limit)
        {
            //Skapa Queryn Här istället.


            var result = new Dictionary<string, List<object>>();

            try
            {
                databaseHandler.dbOpen();


                string query = @"
                                    SELECT 
                                        s.*, 
                                        p.longitud, 
                                        p.latitud, 
                                        p.StartDate
                                    FROM Ships s
                                    JOIN (
                                        SELECT 
                                            MMSI, 
                                            MIN(StartDate) AS FirstStartDate
                                        FROM Positions
                                        WHERE StartDate >= @date AND StartDate <= @dateLimit
                                        GROUP BY MMSI
                                    ) pFirst ON s.MMSI = pFirst.MMSI
                                    JOIN Positions p ON p.MMSI = pFirst.MMSI AND p.StartDate = pFirst.FirstStartDate
                                    WHERE p.longitud BETWEEN @minLng AND @maxLng
                                      AND p.latitud BETWEEN @minLat AND @maxLat LIMIT @shiplimit;";

                using (SQLiteCommand command = new SQLiteCommand(query, (SQLiteConnection)databaseHandler.GetConnector()))
                {
                    command.Parameters.AddWithValue("@minLat", minLat);
                    command.Parameters.AddWithValue("@maxLat", maxLat);
                    command.Parameters.AddWithValue("@minLng", minLng);
                    command.Parameters.AddWithValue("@maxLng", maxLng);
                    command.Parameters.AddWithValue("@date", date);
                    command.Parameters.AddWithValue("@dateLimit", dateLimit);
                    command.Parameters.AddWithValue("@shiplimit", limit);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                if (!result.ContainsKey(columnName))
                                {
                                    result[columnName] = new List<object>();
                                }
                                result[columnName].Add(reader.GetValue(i));
                            }
                        }
                    }
                }
            }
            finally
            {
                databaseHandler.dbClose();
            }
            return result;
        }



        //Hämtar vi alla fartyg som är inom ett viss område.
        public Dictionary<string, List<object>> GetShipInfo(double maxLng, double minLng, double maxLat, double minLat)
        {
       

            var result = new Dictionary<string, List<object>>();

            try
            {
                databaseHandler.dbOpen();
                // longitud
                // latitud
              

                string query = "SELECT * FROM Ships";

                using (SQLiteCommand command = new SQLiteCommand(query, (SQLiteConnection)databaseHandler.GetConnector()))
                {
                    command.Parameters.AddWithValue("@minLng", minLat);
                    command.Parameters.AddWithValue("@maxLat", maxLat);
                    command.Parameters.AddWithValue("@minLng", minLng);
                    command.Parameters.AddWithValue("@maxLng", maxLng);


                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read() && result.Count < 2001) { }
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                if (!result.ContainsKey(columnName))
                                {
                                    result[columnName] = new List<object>();
                                }
                                result[columnName].Add(reader.GetValue(i));
                            }
                        }
                    }
                }
            }
            finally
            {
                databaseHandler.dbClose();
            }
            return result;
        }


        //Flytta över (fast för positioner) till getshitiinfo. 
        //Skapa en privat hjälpmetod för queries

        //Set positions, ta in en CSV fil. Läsa igenom´och lägga till entries som inte existerar, om de är redan med, uppdatera dem. MEN, de måste matcha datumet + tid . 
        public bool SetPositions(string query)
        {
            try
            {
                databaseHandler.dbOpen();

                using (SQLiteCommand command = new SQLiteCommand(query, (SQLiteConnection)databaseHandler.GetConnector()))
                {
                    int affectedRows = command.ExecuteNonQuery();
                    return affectedRows > 0;
                }
            }
            finally
            {
                databaseHandler.dbClose();
            }
        }

        public bool setShipsInfo(string query)
        {
            return SetPositions(query);
        }

    }
}
