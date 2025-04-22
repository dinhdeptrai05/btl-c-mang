using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Client.Core;
using Message = Client.Core.Message;
using Client.enums;

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
        private Label lblTitle;
        private CheckBox chkRememberMe;
        private LinkLabel linkForgotPassword;
        private Panel panelHeader;
        private Button btnClose;
        private Button btnMinimize;

        private Color primaryColor = Color.FromArgb(76, 175, 80);
        private Color secondaryColor = Color.FromArgb(60, 145, 65);
        private Color textColor = Color.FromArgb(60, 60, 60);
        private Color backgroundColor = Color.White;

        public FormLogin()
        {

            InitLoginForm();
            this.Load += FormLogin_Load;

            AuctionClient.gI().RegisterHandler(CommandType.LoginResponse, HandleLoginResponse);
        }

        private void HandleLoginResponse(Message message)
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


        private void InitLoginForm()
        {
            this.BackColor = backgroundColor;

            panelHeader = new Panel();
            panelHeader.Size = new Size(this.Width, 400);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 40;
            panelHeader.BackColor = primaryColor;
            panelHeader.MouseDown += FormLogin_MouseDown;

            btnClose = new Button();
            btnClose.Size = new Size(30, 30);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Text = "✕";
            btnClose.Font = new Font("Arial", 10, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.BackColor = Color.Transparent;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnMinimize = new Button();
            btnMinimize.Size = new Size(30, 30);
            btnMinimize.FlatStyle = FlatStyle.Flat;
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Text = "—";
            btnMinimize.Font = new Font("Arial", 10, FontStyle.Bold);
            btnMinimize.ForeColor = Color.White;
            btnMinimize.BackColor = Color.Transparent;
            btnMinimize.Cursor = Cursors.Hand;
            btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            btnMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            panelHeader.Controls.Add(btnClose);
            panelHeader.Controls.Add(btnMinimize);

            pictureBoxLogo = new PictureBox();
            pictureBoxLogo.Size = new Size(80, 80);
            pictureBoxLogo.Location = new Point((this.Width - 80) / 2 + 50, 60);
            pictureBoxLogo.BackColor = Color.Transparent;
            pictureBoxLogo.Image = CreateLogoImage();
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;

            lblTitle = new Label();
            lblTitle.Text = "TEMPLE";
            lblTitle.Font = new Font("Arial", 20, FontStyle.Bold);
            lblTitle.ForeColor = primaryColor;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(200, 40);
            lblTitle.Location = new Point((this.Width - 200) / 2 + 50, pictureBoxLogo.Bottom + 5);

            lblUsername = new Label();
            lblUsername.Text = "Tên đăng nhập:";
            lblUsername.Font = new Font("Arial", 10);
            lblUsername.ForeColor = textColor;
            lblUsername.Location = new Point(50, lblTitle.Bottom + 30);
            lblUsername.Size = new Size(300, 20);

            txtUsername = new TextBox();
            txtUsername.Location = new Point(50, lblUsername.Bottom + 5);
            txtUsername.Size = new Size(300, 30);
            txtUsername.Font = new Font("Arial", 12);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            lblPassword = new Label();
            lblPassword.Text = "Mật khẩu:";
            lblPassword.Font = new Font("Arial", 10);
            lblPassword.ForeColor = textColor;
            lblPassword.Location = new Point(50, txtUsername.Bottom + 20);
            lblPassword.Size = new Size(300, 20);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(50, lblPassword.Bottom + 5);
            txtPassword.Size = new Size(300, 30);
            txtPassword.Font = new Font("Arial", 12);
            txtPassword.PasswordChar = '•';
            txtPassword.BorderStyle = BorderStyle.FixedSingle;

            chkRememberMe = new CheckBox();
            chkRememberMe.Text = "Ghi nhớ đăng nhập";
            chkRememberMe.Font = new Font("Arial", 9);
            chkRememberMe.ForeColor = textColor;
            chkRememberMe.Location = new Point(50, txtPassword.Bottom + 15);
            chkRememberMe.Size = new Size(150, 20);
            chkRememberMe.Cursor = Cursors.Hand;

            linkForgotPassword = new LinkLabel();
            linkForgotPassword.Text = "Quên mật khẩu?";
            linkForgotPassword.Font = new Font("Arial", 9);
            linkForgotPassword.LinkColor = primaryColor;
            linkForgotPassword.ActiveLinkColor = secondaryColor;
            linkForgotPassword.Location = new Point(240, txtPassword.Bottom + 15);
            linkForgotPassword.Size = new Size(110, 20);
            linkForgotPassword.TextAlign = ContentAlignment.MiddleRight;
            linkForgotPassword.LinkClicked += LinkForgotPassword_LinkClicked;
            linkForgotPassword.Cursor = Cursors.Hand;

            btnLogin = new Button();
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.Font = new Font("Arial", 12, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.BackColor = primaryColor;
            btnLogin.Location = new Point(50, chkRememberMe.Bottom + 30);
            btnLogin.Size = new Size(300, 45);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = secondaryColor;
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = primaryColor;

            this.Controls.Add(panelHeader);
            this.Controls.Add(pictureBoxLogo);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(chkRememberMe);
            this.Controls.Add(linkForgotPassword);
            this.Controls.Add(btnLogin);

            this.Paint += FormLogin_Paint;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            btnClose.Location = new Point(panelHeader.Width - 35, 5);
            btnMinimize.Location = new Point(panelHeader.Width - 70, 5);
        }

        private Image CreateLogoImage()
        {
            Bitmap bitmap = new Bitmap(80, 80);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (SolidBrush brush = new SolidBrush(primaryColor))
                {
                    g.FillEllipse(brush, 5, 5, 70, 70);
                }
                using (Font font = new Font("Arial", 40, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString("T", font, textBrush, new RectangleF(5, 5, 70, 70), sf);

                }
            }
            return bitmap;
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
            using (Pen pen = new Pen(Color.FromArgb(180, 180, 180), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
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
