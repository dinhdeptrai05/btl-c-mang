using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Client.Core;
using Message = Client.Core.Message;

namespace Client
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            try
            {
                Message msg = new Message(1); // 1 là commandId cho đăng nhập
                msg.WriteUTF(username);
                msg.WriteUTF(password);

                string response = Program.Connection.SendMessage(msg);

                MessageBox.Show(response, "Server Response");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error");
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
        }
    }
}