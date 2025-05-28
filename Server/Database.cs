using System;
using System.Data;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace AuctionServer
{
    public class Database
    {
        private static Database _instance; // Instance duy nhất
        private static readonly object _lock = new object(); // Đảm bảo thread-safety
        private readonly string _connectionString;

        // Thông tin kết nối
        private static readonly string server = "18.167.221.122";
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
                init();
            }
        }

        public void init()
        {
            string query = "SELECT * FROM rooms order by is_open desc";
            DataTable result = ExecuteQuery(query);
            foreach (DataRow row in result.Rows)
            {
                // Parse DateTime với multiple formats để tương thích cross-platform
                DateTime auctionStartTime;
                string dateTimeString = row["auction_start_time"].ToString();

                if (!TryParseDateTime(dateTimeString, out auctionStartTime))
                {
                    Console.WriteLine($"Warning: Could not parse datetime '{dateTimeString}', using current time instead");
                    auctionStartTime = DateTime.Now;
                }

                Room room = new Room(
                    int.Parse(row["id"].ToString()),
                    row["name"].ToString(),
                    int.Parse(row["owner_id"].ToString()),
                    User.GetUserById(int.Parse(row["owner_id"].ToString())).Name,
                    int.Parse(row["min_participant"].ToString()),
                    row["time_created"].ToString(),
                    row["items"].ToString(),
                    bool.Parse(int.Parse(row["is_open"].ToString()) == 1 ? "true" : "false"),
                    row["chat"].ToString(),
                    bool.Parse(int.Parse(row["is_started"].ToString()) == 1 ? "true" : "false"),
                    auctionStartTime
                );
                Room.Rooms.Add(room);
            }
            Console.WriteLine("Init rooms thành công");
        }

        // Helper method để parse DateTime với multiple formats
        private bool TryParseDateTime(string dateTimeString, out DateTime result)
        {
            result = DateTime.MinValue;

            // Các định dạng có thể có
            string[] formats = {
                "HH:mm:ss dd/MM/yyyy",
                "dd/MM/yyyy HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "MM/dd/yyyy HH:mm:ss",
                "dd-MM-yyyy HH:mm:ss"
            };

            foreach (string format in formats)
            {
                if (DateTime.TryParseExact(dateTimeString, format,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {
                    return true;
                }
            }

            // Fallback: thử parse với culture hiện tại
            return DateTime.TryParse(dateTimeString, out result);
        }

        // Thực thi truy vấn không trả về kết quả
        public int ExecuteNonQuery(string query, params object[] parameters)
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

                    return command.ExecuteNonQuery();
                }
            }
        }

        // Thực thi truy vấn trả về một giá trị duy nhất
        public object ExecuteScalar(string query, params object[] parameters)
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

                    return command.ExecuteScalar();
                }
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
    }
}