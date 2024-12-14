using System;
using System.Data.Common;
using System.Data.SQLite;

namespace SAAB_Maritime.Model
{
    internal class DatabaseHandler
    {
        private DbConnection sqlConnect;
        private readonly string sqlPath;

        public DatabaseHandler(string connectionString)
        {
            sqlPath = connectionString;
            sqlConnect = new SQLiteConnection(sqlPath);
        }

        public void dbOpen()
        {
            if (sqlConnect.State != System.Data.ConnectionState.Open)
            {
                sqlConnect.Open();
                //Console.WriteLine("Database is open");
            }
        }

        public void dbClose()
        {
            if (sqlConnect.State != System.Data.ConnectionState.Closed)
            {
                sqlConnect.Close();
                //Console.WriteLine("Database is closed");
            }
        }

        public DbConnection GetConnector()
        {
            return sqlConnect;
        }
    }
}
