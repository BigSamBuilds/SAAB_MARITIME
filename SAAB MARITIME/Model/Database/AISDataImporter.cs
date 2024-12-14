using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace SAAB_Maritime.Model.Database
{

    /// <summary>
    /// Handles importing AIS data from CSV files into an SQLite database.
    /// </summary>
    internal class AISDataImporter
    {
        // private readonly string _sampleCSV = "SAMPLE_1hour-vessel-movements-report.csv";
        private readonly string _connectionString;
        private readonly string _dbPath;
        private readonly string _csvFilePath;
        private readonly string _startTime;
        private readonly string _endTime;

        /// <summary>
        /// Initializes an instance of AISDataImporter with the specified parameters.
        /// </summary>
        /// <param name="dbPath">The path to the SQLite database.</param>
        /// <param name="date">The date associated with the import data (format: yyyy-MM-dd).</param>
        /// <param name="startTime">The start time for filtering data (format: HH:mm:ss).</param>
        /// <param name="endTime">The end time for filtering data (format: HH:mm:ss).</param>
        /// <param name="csvFileName">The name of the CSV file containing AIS data.</param>
        public AISDataImporter(string dbPath, string date = "2024-04-12", string startTime = "00:00:00", string endTime = "23:59:59", string csvFileName = "aisdk-2024-04-12.csv")
        {
            _dbPath = dbPath;
            _csvFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "Model", "Database", csvFileName);
            _connectionString = $"Data Source={_dbPath}; Version = 3;";
            _startTime = $"{date} {startTime}";
            _endTime = $"{date} {endTime}";
        }

        /// <summary>
        /// Imports data from the specified CSV file into the SQLite database.
        /// </summary>
        public void ImportData()
        {
            Console.WriteLine("[CONSOLE MESSAGE] Importing data into tables from: " + _csvFilePath);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var insertShipQuery = CreateInsertShipQuery(connection);
                    var insertPositionQuery = CreateInsertPositionQuery(connection);

                    using (var reader = new StreamReader(_csvFilePath))
                    {
                        reader.ReadLine(); // Skip header
                        ProcessFile(reader, insertShipQuery, insertPositionQuery);

                    }
                    transaction.Commit();
                }
            }
            Console.WriteLine("[CONSOLE MESSAGE] Data imported into Ships and Positions tables at: " + _dbPath);
        }

        /// <summary>
        /// Adds a single ship into the database.
        /// </summary>
        /// <param name="dbPath">The path to the SQLite database.</param>
        /// <param name="MMSI">The unique MMSI number of the ship.</param>
        /// <param name="name">The name of the ship (default: empty string).</param>
        /// <param name="model">The model of the ship (default: "Undefined").</param>
        public void importShip(int MMSI, string name = " ", string model = "Undefined")
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = CreateInsertShipQuery(connection))
                    {
                        // Add parameter values for the ship's details
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@MMSI", MMSI);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@modell", model);
                        
                        // Execute the insert command
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"[CONSOLE ERROR] SQLite error occurred while adding ship: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONSOLE ERROR] An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the SQLite command for inserting records into the Ships table.
        /// </summary>
        /// <param name="connection">The active SQLite connection.</param>
        /// <returns>A prepared SQLiteCommand for the Ships table.</returns>
        private SQLiteCommand CreateInsertShipQuery(SQLiteConnection connection)
        {
            return new SQLiteCommand(@"
                INSERT OR REPLACE INTO Ships (MMSI, namn, modell)
                VALUES (@MMSI, @namn, @modell);
            ", connection);
        }

        /// <summary>
        /// Creates the SQLite command for inserting records into the Positions table.
        /// </summary>
        /// <param name="connection">The active SQLite connection.</param>
        /// <returns>A prepared SQLiteCommand for the Positions table.</returns>
        private SQLiteCommand CreateInsertPositionQuery(SQLiteConnection connection)
        {
            return new SQLiteCommand(@"
                INSERT OR IGNORE INTO Positions(MMSI, radio_Callsign, longitud, latitud, course, speed, StartDate, ETA)
                VALUES(@MMSI, @radio_Callsign, @longitud, @latitud, @course, @speed, @StartDate, @ETA);
            ", connection);
        }

        /// <summary>
        /// Processes the CSV file and imports data into the database.
        /// </summary>
        /// <param name="reader">A StreamReader object for reading the CSV file.</param>
        /// <param name="insertShipQuery">A prepared SQLiteCommand for inserting into the Ships table.</param>
        /// <param name="insertPositionQuery">A prepared SQLiteCommand for inserting into the Positions table.</param>
        /// <param name="maxIterations">The maximum number of records to process (default: unlimited).</param>
        private void ProcessFile(StreamReader reader, SQLiteCommand insertShipQuery, SQLiteCommand insertPositionQuery, int maxIterations = int.MaxValue)
        {
            int iterations = 0;
            var stopwatch = Stopwatch.StartNew();

            while (!reader.EndOfStream && iterations < maxIterations)
            {
                try
                {
                    var values = reader.ReadLine().Split(',');
                    string recordTime = ParseDate(values[0]).ToString();


                    if (string.Compare(recordTime, _startTime) < 0)
                    {
                        Console.WriteLine("[CONSOLE MESSAGE] Skipping Line: " + string.Join(",", values));

                        continue;
                    }
                    if (string.Compare(recordTime, _endTime) > 0)
                    {
                        Console.WriteLine($"[DEBUG] Reached record beyond end time: {recordTime}. Stopping further processing.");
                        break;
                    }
                    Console.WriteLine("[CONSOLE MESSAGE] Importing Line: " + string.Join(",", values));

                    ImportLine(values, insertShipQuery, insertPositionQuery);
                    iterations++;

                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"[CONSOLE WARNING] Skipping line due to format error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CONSOLE ERROR] An unexpected error occurred: {ex.Message}");
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Total processing time: {stopwatch.Elapsed}");
            Console.WriteLine("[CONSOLE MESSAGE] " + iterations + " records processed.");
        }

        /// <summary>
        /// Imports a single record into the database.
        /// </summary>
        /// <param name="values">The array of CSV values.</param>
        /// <param name="insertShipQuery">A prepared SQLiteCommand for inserting into the Ships table.</param>
        /// <param name="insertPositionQuery">A prepared SQLiteCommand for inserting into the Positions table.</param>
        private void ImportLine(string[] values, SQLiteCommand insertShipQuery, SQLiteCommand insertPositionQuery)
        {
            int mmsi = Convert.ToInt32(values[2]);
            string name = values[12];
            string model = values[13];
            string callSign = values[11];

            double longitude = ParseDouble(values[4], "Longitude");
            double latitude = ParseDouble(values[3], "Latitude");
            double course = ParseDouble(values[8], "Course", defaultValue: 0);
            double speed = ParseDouble(values[7], "Speed", defaultValue: 0);
            object startDate = ParseDate(values[0]);
            string eta = string.IsNullOrWhiteSpace(values[20]) ? "Unknown" : values[20];

            ExecuteInsert(insertShipQuery, mmsi, name, model);
            ExecuteInsert(insertPositionQuery, mmsi, callSign, longitude, latitude, course, speed, startDate, eta);
        }

        /// <summary>
        /// Parses a string value into a double.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <param name="fieldName">The name of the field being parsed (for debugging).</param>
        /// <param name="defaultValue">The default value to use if parsing fails.</param>
        /// <returns>The parsed double value.</returns>
        private double ParseDouble(string value, string fieldName, double defaultValue = double.NaN)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                switch (fieldName)
                {
                    case "Course":
                        //Console.WriteLine($"[DEBUG WARNING] {fieldName} is empty, using default value: 0");
                        return 0;

                    case "Speed":
                        //Console.WriteLine($"[DEBUG WARNING] {fieldName} is empty, using default value: 0");
                        return 0;

                    default:
                        throw new FormatException($"[DEBUG WARNING] Missing or empty value for critical field: '{fieldName}'");
                }
            }

            return double.TryParse(value.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw new FormatException($"[DEBUG WARNING] Invalid {fieldName}: '{value}'");
        }

        /// <summary>
        /// Parses a date string into a DateTime object.
        /// </summary>
        /// <param name="rawDate">The raw date string.</param>
        /// <returns>A formatted DateTime object or DBNull.Value if parsing fails.</returns>
        private object ParseDate(string rawDate)
        {
            if (DateTime.TryParseExact(rawDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtStartDate))
            {
                return dtStartDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                return DBNull.Value; // Use DBNull.Value if StartDate is invalid
            }
        }

        /// <summary>
        /// Executes an insert command for the Ships table.
        /// </summary>
        /// <param name="command">The prepared SQLite command.</param>
        /// <param name="mmsi">The MMSI number.</param>
        /// <param name="name">The ship name.</param>
        /// <param name="model">The ship model.</param>
        private void ExecuteInsert(SQLiteCommand command, int mmsi, string name, string model)
        {
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@MMSI", mmsi);
            command.Parameters.AddWithValue("@namn", name);
            command.Parameters.AddWithValue("@modell", model);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes an insert command for the Positions table.
        /// </summary>
        /// <param name="command">The prepared SQLite command.</param>
        /// <param name="mmsi">The MMSI number.</param>
        /// <param name="callSign">The radio call sign.</param>
        /// <param name="longitude">The ship's longitude.</param>
        /// <param name="latitude">The ship's latitude.</param>
        /// <param name="course">The ship's course.</param>
        /// <param name="speed">The ship's speed.</param>
        /// <param name="startDate">The start date of the record.</param>
        /// <param name="eta">The estimated time of arrival.</param>
        private void ExecuteInsert(SQLiteCommand command, int mmsi, string callSign, double longitude, double latitude, double course, double speed, object startDate, string eta)
        {
            command.Parameters.Clear();
            command.Parameters.AddWithValue("@MMSI", mmsi);
            command.Parameters.AddWithValue("@radio_Callsign", callSign ?? string.Empty);
            command.Parameters.AddWithValue("@longitud", longitude > 90 ? DBNull.Value : (object)longitude);
            command.Parameters.AddWithValue("@latitud", latitude > 90 ? DBNull.Value : (object)latitude);
            command.Parameters.AddWithValue("@course", course);
            command.Parameters.AddWithValue("@speed", speed);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@ETA", eta);
            command.ExecuteNonQuery();
        }
    }
}
