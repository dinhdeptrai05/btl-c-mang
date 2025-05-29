using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Client.Forms.Register;
using Message = Client.Core.Message;
namespace Client.Forms.Login
{
    public partial class FormLogin : Form
    {
        public static FormLogin instance;

        // Màu sắc với tông màu chủ đạo là xanh lá
        private Color primaryColor = Color.FromArgb(76, 175, 80);     // Xanh lá
        private Color secondaryColor = Color.FromArgb(60, 145, 65);   // Xanh lá đậm
        private Color backgroundColor = Color.White;                  // Nền trắng
        private Color textFieldColor = Color.FromArgb(236, 247, 250); // Màu nền textbox xanh nhạt
        private Color borderColor = Color.FromArgb(200, 228, 238);    // Màu viền xanh nhạt
        private Color textColor = Color.FromArgb(90, 90, 90);

        public static FormLogin gI()
        {
            return instance;
        }

        public FormLogin()
        {
            instance = this;
            InitializeComponent();

            // Đăng ký xử lý sự kiện đăng nhập
            AuctionClient.gI().RegisterHandler(CommandType.LoginResponse, HandleLoginResponse);
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            // Thiết lập màu sắc cho các control
            this.BackColor = Color.FromArgb(240, 240, 240);
            panelMain.BackColor = backgroundColor;
            btnLogin.BackColor = Color.FromArgb(74, 174, 211);
            btnLogin.ForeColor = Color.White;
            txtUsername.BackColor = textFieldColor;
            txtPassword.BackColor = textFieldColor;
            txtUsername.ForeColor = textColor;
            txtPassword.ForeColor = textColor;
            lblUsername.ForeColor = primaryColor;
            lblPassword.ForeColor = primaryColor;

            // Thiết lập logo
            pictureBoxLogo.Image = CreateLogoImage(120);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
        }
        private void FormLogin_Resize(object sender, EventArgs e)
        {
            UpdateControlPositions();
        }

        private void UpdateControlPositions()
        {
            // Cập nhật vị trí của panel chính để luôn ở giữa form
            panelMain.Location = new Point((this.ClientSize.Width - panelMain.Width) / 2, (this.ClientSize.Height - panelMain.Height) / 2);
        }

        private Image CreateLogoImage(int size)
        {
            int padding = size / 10;
            Bitmap bitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Vẽ hình tròn xanh nhạt như trong hình mẫu
                using (SolidBrush circleBrush = new SolidBrush(Color.FromArgb(173, 222, 241)))
                {
                    g.FillEllipse(circleBrush, padding, padding, size - 2 * padding, size - 2 * padding);
                }

                // Vẽ biểu tượng nhân vật đơn giản màu trắng giống hình mẫu
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    // Vẽ hình đầu và cơ thể
                    int headSize = (int)(size * 0.5);
                    int headTop = (int)(size * 0.3);
                    g.FillEllipse(whiteBrush, (size - headSize) / 2, headTop, headSize, headSize);

                    // Vẽ chữ T cho logo TEMPLE
                    using (Font font = new Font("Arial", size / 5, FontStyle.Bold))
                    {
                        System.Drawing.StringFormat sf = new System.Drawing.StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };
                        g.DrawString("T", font, new SolidBrush(primaryColor),
                            new RectangleF(0, size / 2 - size / 8, size, size / 4), sf);
                    }
                }
            }
            return bitmap;
        }

        public void HandleLoginResponse(Message message)
        {
            bool success = message.ReadBoolean();
            Console.WriteLine($"Login response: {success}");

            if (success)
            {
                int userId = message.ReadInt();
                string username = message.ReadUTF();
                string name = message.ReadUTF();
                string avatarUrl = message.ReadUTF();

                // Lưu userId nếu cần
                AuctionClient.gI().UserId = userId;
                AuctionClient.gI().Username = username;
                AuctionClient.gI().Name = name;

                if (avatarUrl != null)
                {
                    AuctionClient.gI().avatar_url = avatarUrl;
                }
                else
                {
                    AuctionClient.gI().avatar_url = "https://www.w3schools.com/howto/img_avatar.png";
                }

                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Đăng nhập thành công!\nXin chào {name} (ID: {userId})",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Mở form chính tại đây nếu muốn
                    this.Hide();
                    new FormLobby().Show();
                }));
            }
            else
            {
                string error = message.ReadUTF();
                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Đăng nhập thất bại. {error}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Message msg = new Message(CommandType.Login);
            msg.WriteUTF(username);
            msg.WriteUTF(password);
            AuctionClient.SendMessage(msg);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        private void FormLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            new FormRegister().Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblUsername_Click(object sender, EventArgs e)
        {

        }

        private void lblPassword_Click(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }

    internal static class NativeMethods
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}