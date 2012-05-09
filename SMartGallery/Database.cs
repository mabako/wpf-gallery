using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SMartGallery
{
    /// <summary>
    /// SQLite3-Connector
    /// </summary>
    class Database : IDisposable
    {
        /// <summary>
        /// the actual connection
        /// </summary>
        private static SQLiteConnection connection;

        /// <summary>
        /// Prepared Statement for selecting a random picture, which needs to be 'not too bad' in terms of how often it has been chosen before
        /// </summary>
        private static SQLiteCommand qRandomPicture;
        
        /// <summary>
        /// Prepared Statement to insert a picture
        /// </summary>
        private SQLiteCommand qInsertPicture;

        /// <summary>
        /// Path of the picture
        /// </summary>
        private SQLiteParameter qInsertPath;
        
        /// <summary>
        /// Prepared Statement for an 'up vote'
        /// </summary>
        private static SQLiteCommand qVoteUp;

        /// <summary>
        /// Path of the picture to upvote
        /// </summary>
        private static SQLiteParameter qVoteUpPath;

        /// <summary>
        /// Prepared Statement for a 'down vote'
        /// </summary>
        private static SQLiteCommand qVoteDown;

        /// <summary>
        /// Path of the picture to downvote
        /// </summary>
        private static SQLiteParameter qVoteDownPath;

        /// <summary>
        /// Connects to a SQLite3-Database
        /// </summary>
        /// <param name="path">Path of the Database</param>
        public Database(string path)
        {
            connection = new SQLiteConnection();
            connection.ConnectionString = "Data Source=" + path + ";Version=3;New=True;Compress=True";
            connection.Open();

            // Create required databases
            createTable("pictures", "path VARCHAR(255) PRIMARY KEY NOT NULL, up INT NOT NULL DEFAULT 0, down INT NOT NULL DEFAULT 0");

            // Prepare prepared statements, duh.
            qRandomPicture = new SQLiteCommand(connection);
            qRandomPicture.CommandText = "SELECT path FROM pictures WHERE (up - down) > (SELECT AVG(up-down)-2 FROM pictures) ORDER BY RANDOM() LIMIT 1";

            qInsertPicture = new SQLiteCommand(connection);
            qInsertPicture.CommandText = "INSERT OR IGNORE INTO pictures (path) VALUES (@path)";
            qInsertPath = qInsertPicture.CreateParameter();
            qInsertPath.ParameterName = "@path";
            qInsertPicture.Parameters.Add(qInsertPath);

            qVoteUp = new SQLiteCommand(connection);
            qVoteUp.CommandText = "UPDATE pictures SET up = up + 1 WHERE path = @path";
            qVoteUpPath = qVoteUp.CreateParameter();
            qVoteUpPath.ParameterName = "@path";
            qVoteUp.Parameters.Add(qVoteUpPath);

            qVoteDown = new SQLiteCommand(connection);
            qVoteDown.CommandText = "UPDATE pictures SET down = down + 1 WHERE path = @path";
            qVoteDownPath = qVoteDown.CreateParameter();
            qVoteDownPath.ParameterName = "@path";
            qVoteDown.Parameters.Add(qVoteDownPath);
        }

        /// <summary>
        /// Creates a new SQL Table.
        /// </summary>
        /// <param name="tableName">Name of the Table</param>
        /// <param name="commandText">commands for field definition</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        private void createTable(string tableName, string commandText)
        {
            SQLiteCommand command = null;
            try
            {
                command = new SQLiteCommand(connection);
                command.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName + " (" + commandText + ")";
                command.ExecuteNonQuery();
            }
            finally
            {
                command.Dispose();
            }
        }

        /// <summary>
        /// Returns a random picture not being the one given in ignore1, ignore2
        /// </summary>
        /// <param name="ignore1">(not implemented)</param>
        /// <param name="ignore2">(not implemented)</param>
        /// <returns></returns>
        public static string getRandomPicture(string ignore1, string ignore2)
        {
            for (int i = 0; i < 10; ++i)
            {
                //qRandomIgnore1.Value = ignore1;
                //qRandomIgnore2.Value = ignore2;
                string path = (string)qRandomPicture.ExecuteScalar();
                if (path != null && !path.Equals(ignore1) && !path.Equals(ignore2))
                    return path;
            }
            return null;
        }

        /// <summary>
        /// Adds a picture to the database.
        /// </summary>
        /// <param name="path">path of the picture</param>
        public void insert(string path)
        {
            qInsertPath.Value = path;
            qInsertPicture.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates a picture, either up or down
        /// </summary>
        /// <param name="path">path of the picture</param>
        /// <param name="what">up-vote if 'up', down-vote otherwise</param>
        public static void update(string path, string what)
        {
            if (what.Equals("up"))
            {
                qVoteUpPath.Value = path;
                qVoteUp.ExecuteNonQuery();
            }
            else
            {
                qVoteDownPath.Value = path;
                qVoteDown.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Disposes the connection and all queries.
        /// </summary>
        public void Dispose()
        {
            qRandomPicture.Dispose();
            qInsertPicture.Dispose();
            qVoteUp.Dispose();
            qVoteDown.Dispose();

            connection.Dispose();
        }
    }
}
