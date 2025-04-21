using System;
using System.Windows.Forms;
using Client.Core;

namespace Client
{
    static class Program
    {
        public static ClientConnection Connection; // Biến toàn cục để lưu kết nối

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi tạo kết nối tới server
            try
            {
                Connection = new ClientConnection("127.0.0.1", 8000);
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server: {ex.Message}", "Error");
            }
            finally
            {
                Connection?.Dispose();
            }
        }
    }
}