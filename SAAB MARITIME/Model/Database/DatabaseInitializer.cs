using System;
using System.Data.SQLite;
using System.IO;

namespace SAAB_Maritime.Model.Database
{
    /// <summary>
    /// Initializes and manages the SQLite database for AIS data storage.
    /// </summary>
    internal class DatabaseInitializer
    {
        private readonly string _dbPath;

        /// <summary>
        /// Constructor to initialize the database.
        /// </summary>
        /// <param name="date">The date associated with the database (format: yyyy-MM-dd).</param>
        /// <param name="startTime">The start time for the data range (format: HH:mm:ss).</param>
        /// <param name="endTime">The end time for the data range (format: HH:mm:ss).</param>
        /// <param name="optionalPath">An optional path name for the database.</param>
        /// <param name="saveToDisk">Boolean indicating whether the database should be saved to disk.</param>
        public DatabaseInitializer(string date = "2024-04-12", string startTime = "00:00:00", string endTime = "23:59:59", string optionalPath = "AIS_DATA", Boolean saveToDisk = false)
        {
            _dbPath = SetDataBasePath(date, startTime, endTime, optionalPath, saveToDisk);
            InitializeDatabase();
            CreateTables();
            ListExistingTables();
        }

        /// <summary>
        /// Sets the path for the database file.
        /// </summary>
        /// <param name="date">The date associated with the database (format: yyyy-MM-dd).</param>
        /// <param name="startTime">The start time for the data range (format: HH:mm:ss).</param>
        /// <param name="endTime">The end time for the data range (format: HH:mm:ss).</param>
        /// <param name="optionalPath">An optional path name for the database.</param>
        /// <param name="saveToDisk">Boolean indicating whether the database should be saved to disk.</param>
        /// <returns>The full path to the database file.</returns>
        private string SetDataBasePath(string date, string startTime, string endTime, string optionalPath, Boolean saveToDisk)
        {
            string sanitizedStartTime = startTime.Replace(":", "_");
            string sanitizedEndTime = endTime.Replace(":", "_");

            string dbFileName = date + "_" + sanitizedStartTime + "-" + sanitizedEndTime + "_AIS_DATA.db";

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(currentDirectory, "Model", "Database", dbFileName);


            var targetDirectory = Path.GetDirectoryName(dbPath);

            if (saveToDisk) { dbPath = Path.Combine(Directory.GetParent(currentDirectory).Parent.Parent.FullName, "Model", "Database", dbFileName); }

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                Console.WriteLine("[CONSOLE MESSAGE] Database file created at: " + dbPath);
            }

            return dbPath;
        }
        
        /// <summary>
        /// Creates the required tables in the database.
        /// </summary>
        private void CreateTables()
        {
            using (var connection = new SQLiteConnection($"Data Source={_dbPath}; Version = 3;"))
            {
                connection.Open();
                CreatePositionTableQuery(connection);
                CreateShipsTableQuery(connection);
                Console.WriteLine("[CONSOLE MESSAGE] Database tables created succesfully.");
            }
        }

        /// <summary>
        /// Creates the Ships table in the database.
        /// </summary>
        /// <param name="connection">The active SQLite connection.</param>
        private void CreateShipsTableQuery(SQLiteConnection connection)
        {
            string shipsTableQuery = @"CREATE TABLE IF NOT EXISTS Ships (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MMSI INTEGER NOT NULL UNIQUE,
                    namn TEXT(255),
                    modell TEXT(255)
                    );";
            using (var command = new SQLiteCommand(shipsTableQuery, connection)) { command.ExecuteNonQuery(); }

        }

        /// <summary>
        /// Creates the Positions table in the database.
        /// </summary>
        /// <param name="connection">The active SQLite connection.</param>
        private void CreatePositionTableQuery(SQLiteConnection connection)
        {
            string positionTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Positions (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        MMSI INTEGER NOT NULL,
                        radio_Callsign TEXT,
                        longitud REAL,
                        latitud REAL,
                        course REAL,
                        speed REAL,
                        StartDate TEXT,
                        ShipRealTimeDate TIMESTAMP,
                        ETA TEXT,
                        FOREIGN KEY (MMSI) REFERENCES Ships(MMSI),
                        UNIQUE (MMSI, StartDate, longitud, latitud)
                    );";
            using (var command = new SQLiteCommand(positionTableQuery, connection)) { command.ExecuteNonQuery(); }

        }

        /// <summary>
        /// Initializes the database by creating the file if it doesn't exist.
        /// </summary>
        private void InitializeDatabase()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }
            Console.Write("[CONSOLE MESSAGE] Database initialized at: " + _dbPath);
        }

        /// <summary>
        /// Lists all existing tables in the database.
        /// </summary>
        private void ListExistingTables()
        {
            using (var connection = new SQLiteConnection($"Data Source = {_dbPath}; Version = 3;"))
            {
                connection.Open();

                string listTableQuery = "SELECT name FROM sqlite_master WHERE type = 'table';";

                using (var command = new SQLiteCommand(listTableQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("[CONSOLE MESSAGE] Existing tables in database: ");
                    while (reader.Read()) { Console.WriteLine(" - " + reader.GetString(0)); }
                }
            }
        }
        
        /// <summary>
        /// Gets the path to the database file.
        /// </summary>
        /// <returns>The path to the database file.</returns>
        public string GetdbPath()
        {
            return _dbPath;
        }

    }
}