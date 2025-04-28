using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace AuctionServer
{
    public class Database
    {
        private static Database _instance; // Instance duy nhất
        private static readonly object _lock = new object(); // Để đảm bảo thread-safety
        private readonly string _connectionString;

        // Thông tin kết nối
        private static readonly string server = "localhost";
        private static readonly string database = "btl_mang";
        private static readonly string username = "root";
        private static readonly string password = "imaira2003";

        // Constructor private để ngăn việc khởi tạo từ bên ngoài
        private Database()
        {
            _connectionString = $"Server={server};Database={database};User={username};Password={password};";
        }

        // Phương thức để lấy instance duy nhất
        public static Database gI()
        {
            if (_instance == null)
            {
                lock (_lock) // Đảm bảo thread-safety
                {
                    if (_instance == null)
                    {
                        _instance = new Database();
                    }
                }
            }
            return _instance;
        }

        // Phương thức để lấy kết nối MySQL
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // Kiểm tra kết nối
        public void TestConnection()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                Console.WriteLine("Kết nối tới MySQL thành công!");
            }
        }

        // Thực thi truy vấn trả về DataTable
        public DataTable ExecuteQuery(string query, params object[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@param{i}", parameters[i]);
                        }
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

        // Thực thi truy vấn không trả về kết quả
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

        // Thực thi truy vấn trả về một giá trị duy nhất
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