using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Message = Client.Core.Message;

namespace Client.Forms
{
    public partial class FormLogin : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblUsername;
        private Label lblPassword;
        private PictureBox pictureBoxLogo;
        private Panel panelMain;
        private Panel panelContent;

        // Màu sắc với tông màu chủ đạo là xanh lá
        private Color primaryColor = Color.FromArgb(76, 175, 80);     // Xanh lá
        private Color secondaryColor = Color.FromArgb(60, 145, 65);   // Xanh lá đậm
        private Color backgroundColor = Color.White;                  // Nền trắng
        private Color textFieldColor = Color.FromArgb(236, 247, 250); // Màu nền textbox xanh nhạt
        private Color borderColor = Color.FromArgb(200, 228, 238);    // Màu viền xanh nhạt
        private Color textColor = Color.FromArgb(90, 90, 90);

        public static FormLogin instance;

        public static FormLogin gI()
        {
            return instance;
        }

        public FormLogin()
        {
            instance = this;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 550);
            this.BackColor = Color.FromArgb(240, 240, 240); // Nền xám nhạt cho form
            this.DoubleBuffered = true;
            this.MinimumSize = new Size(350, 500); // Kích thước tối thiểu để đảm bảo responsive
            InitLoginForm();
            this.Load += FormLogin_Load;
            this.Resize += FormLogin_Resize;

            AuctionClient.gI().RegisterHandler(CommandType.LoginResponse, HandleLoginResponse);


        }

        private void InitLoginForm()
        {
            // Panel chính - Phần trắng ở giữa
            panelMain = new Panel();
            panelMain.BackColor = backgroundColor;
            panelMain.Size = new Size(340, 480);
            panelMain.Location = new Point((this.ClientSize.Width - panelMain.Width) / 2, (this.ClientSize.Height - panelMain.Height) / 2);
            panelMain.Anchor = AnchorStyles.None;
            this.Controls.Add(panelMain);

            // Panel chứa nội dung - Để căn chỉnh tất cả các thành phần bên trong
            panelContent = new Panel();
            panelContent.BackColor = Color.Transparent;
            panelContent.Size = new Size(280, 420);
            panelContent.Location = new Point((panelMain.Width - panelContent.Width) / 2, 30);
            panelMain.Controls.Add(panelContent);

            // Logo TEMPLE
            pictureBoxLogo = new PictureBox();
            pictureBoxLogo.Size = new Size(120, 120);
            pictureBoxLogo.Location = new Point((panelContent.Width - 120) / 2, 10);
            pictureBoxLogo.BackColor = Color.Transparent;
            pictureBoxLogo.Image = CreateLogoImage(120);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            panelContent.Controls.Add(pictureBoxLogo);

            // Label Email (sử dụng để hiển thị "Tên đăng nhập" thay vì "Email")
            lblUsername = new Label();
            lblUsername.Text = "Email";
            lblUsername.Font = new Font("Segoe UI", 10);
            lblUsername.ForeColor = primaryColor;
            lblUsername.Size = new Size(280, 20);
            lblUsername.Location = new Point(0, pictureBoxLogo.Bottom + 30);
            panelContent.Controls.Add(lblUsername);

            // TextBox Email
            txtUsername = new TextBox();
            txtUsername.Size = new Size(280, 40);
            txtUsername.Location = new Point(0, lblUsername.Bottom + 5);
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.ForeColor = textColor;
            txtUsername.BackColor = textFieldColor;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            //   txtUsername.PlaceholderText = "email@domain.com"
            panelContent.Controls.Add(txtUsername);

            // Label Mật khẩu
            lblPassword = new Label();
            lblPassword.Text = "Password";
            lblPassword.Font = new Font("Segoe UI", 10);
            lblPassword.ForeColor = primaryColor;
            lblPassword.Size = new Size(280, 20);
            lblPassword.Location = new Point(0, txtUsername.Bottom + 20);
            panelContent.Controls.Add(lblPassword);

            // TextBox Mật khẩu
            txtPassword = new TextBox();
            txtPassword.Size = new Size(280, 40);
            txtPassword.Location = new Point(0, lblPassword.Bottom + 5);
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.ForeColor = textColor;
            txtPassword.BackColor = textFieldColor;
            txtPassword.PasswordChar = '•';
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            panelContent.Controls.Add(txtPassword);

            // Nút Đăng nhập
            btnLogin = new Button();
            btnLogin.Text = "Log in";
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.BackColor = Color.FromArgb(74, 174, 211); // Màu xanh da trời như trong hình
            btnLogin.Size = new Size(280, 45);
            btnLogin.Location = new Point(0, txtPassword.Bottom + 30);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(64, 164, 201);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(74, 174, 211);
            panelContent.Controls.Add(btnLogin);

            // Button đóng form (X)
            Button btnClose = new Button();
            btnClose.Size = new Size(30, 30);
            btnClose.Location = new Point(panelMain.Width - 35, 5);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Text = "✕";
            btnClose.Font = new Font("Arial", 10, FontStyle.Bold);
            btnClose.ForeColor = Color.FromArgb(150, 150, 150);
            btnClose.BackColor = Color.Transparent;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.FromArgb(100, 100, 100);
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.FromArgb(150, 150, 150);
            panelMain.Controls.Add(btnClose);

            // Thêm sự kiện để di chuyển form
            panelMain.MouseDown += FormLogin_MouseDown;
            this.MouseDown += FormLogin_MouseDown;

            // Đổ bóng cho form chính
            this.Paint += FormLogin_Paint;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            UpdateControlPositions();
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
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                // Vẽ hình tròn xanh nhạt như trong hình mẫu
                using (SolidBrush circleBrush = new SolidBrush(Color.FromArgb(173, 222, 241)))
                {
                    g.FillEllipse(circleBrush, padding, padding, size - 2 * padding, size - 2 * padding);
                }

                // Vẽ biểu tượng nhân vật đơn giản màu trắng giống hình mẫu
                // Tạo hình dạng nhân vật đơn giản
                using (Pen outlinePen = new Pen(Color.White, 3))
                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                {
                    // Vẽ hình đầu và cơ thể
                    int headSize = (int)(size * 0.5);
                    int headTop = (int)(size * 0.3);
                    g.FillEllipse(whiteBrush, (size - headSize) / 2, headTop, headSize, headSize);

                    // Vẽ tóc và các chi tiết khác bằng Pen
                    // Điều này chỉ là một biểu tượng đơn giản, thực tế logo TEMPLE sẽ được thay thế

                    // Vẽ chữ T cho logo TEMPLE (nhưng được ẩn trong trường hợp này)
                    using (Font font = new Font("Arial", size / 5, FontStyle.Bold))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString("T", font, new SolidBrush(primaryColor), new RectangleF(0, size / 2 - size / 8, size, size / 4), sf);
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

                // Lưu userId nếu cần
                // AuctionClient.gI().UserId = userId;

                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Đăng nhập thành công!\nXin chào {username} (ID: {userId})", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Mở form chính tại đây nếu muốn
                    // this.Hide(); new MainForm().Show();
                }));
            }
            else
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show("Đăng nhập thất bại. Sai tài khoản hoặc mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Message msg = new Message(1);
            msg.WriteUTF(username);
            msg.WriteUTF(password);
            AuctionClient.SendMessage(msg);

        }

        private void LinkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Chức năng đang phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FormLogin_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(this.Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        private void FormLogin_Paint(object sender, PaintEventArgs e)
        {
            // Tạo hiệu ứng đổ bóng cho form
            ControlPaint.DrawBorder(e.Graphics, panelMain.Bounds,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(200, 200, 200), 1, ButtonBorderStyle.Solid);
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
