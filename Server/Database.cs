using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace AuctionServer
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string server, string database, string username, string password)
        {
            _connectionString = $"Server={server};Database={database};User={username};Password={password};";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public void TestConnection()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                Console.WriteLine("Kết nối tới MySQL thành công!");
            }
        }

        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteScalar();
                }
            }
        }
    }
}