using System;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Client.Forms.Login;
using Message = Client.Core.Message;

namespace Client.Forms.Register
{
    public partial class FormRegister : Form
    {
        public static FormRegister instance;


        public static FormRegister gI()
        {
            return instance;
        }

        public FormRegister()
        {
            instance = this;
            InitializeComponent();

            AuctionClient.gI().RegisterHandler(CommandType.RegisterResponse, HandleRegisterResponse);

        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            string name = this.NameBox.Text.Trim();
            string username = this.UsernameBox.Text.Trim();
            string password = this.PasswordBox.Text;


            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            Message msg = new Message(CommandType.Register);
            msg.WriteUTF(name);
            msg.WriteUTF(username);
            msg.WriteUTF(password);
            AuctionClient.SendMessage(msg);
        }

        public void HandleRegisterResponse(Message message)
        {
            bool success = message.ReadBoolean();
            Console.WriteLine($"Register response: {success}");

            if (success)
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Đăng ký thành công!)",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    new FormLogin().Show();
                }));
            }
            else
            {
                string errorMessage = message.ReadUTF();
                Invoke(new Action(() =>
                {
                    MessageBox.Show($"{errorMessage}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void PasswordBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            new FormLogin().Show();
        }

        private void UsernameBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
