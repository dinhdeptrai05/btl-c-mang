using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Message = Client.Core.Message;

namespace Client.Forms.Lobby
{
    public partial class FormProfile : Form
    {
        private static FormProfile instance;
        private TextBox nameBox;
        private TextBox usernameBox;
        private TextBox passwordBox;
        private PictureBox avatarBox;
        private Button saveButton;
        private Button cancelButton;
        private Button changeAvatarButton;
        private string currentAvatarUrl;
        private string selectedImagePath;

        public static FormProfile gI()
        {
            return instance;
        }

        public FormProfile()
        {
            instance = this;
            InitializeCustomComponents();
            LoadUserData();
            AuctionClient.gI().RegisterHandler(CommandType.UpdateProfileResponse, HandleUpdateProfileResponse);

        }

        private void InitializeCustomComponents()
        {
            // Form settings
            this.Text = "Cài đặt hồ sơ";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(35, 35, 50);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Avatar section
            avatarBox = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point((this.Width - 150) / 2, 20),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle
            };

            changeAvatarButton = new Button
            {
                Text = "Thay đổi ảnh đại diện",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point((this.Width - 200) / 2, 180),
                Size = new Size(200, 35),
                Font = new Font("Segoe UI", 10)
            };
            changeAvatarButton.Click += ChangeAvatarButton_Click;

            // Name input
            Label nameLabel = new Label
            {
                Text = "Họ và tên:",
                ForeColor = Color.White,
                Location = new Point(50, 240),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            nameBox = new TextBox
            {
                Location = new Point(50, 270),
                Size = new Size(400, 30),
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };

            // Username input
            Label usernameLabel = new Label
            {
                Text = "Tên đăng nhập:",
                ForeColor = Color.White,
                Location = new Point(50, 320),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            usernameBox = new TextBox
            {
                Location = new Point(50, 350),
                Size = new Size(400, 30),
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };

            // Password input
            Label passwordLabel = new Label
            {
                Text = "Mật khẩu mới:",
                ForeColor = Color.White,
                Location = new Point(50, 400),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            passwordBox = new TextBox
            {
                Location = new Point(50, 430),
                Size = new Size(400, 30),
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            // Buttons
            saveButton = new Button
            {
                Text = "Lưu thay đổi",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(50, 490),
                Size = new Size(190, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "Hủy",
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(260, 490),
                Size = new Size(190, 40),
                Font = new Font("Segoe UI", 10)
            };
            cancelButton.Click += CancelButton_Click;

            // Add controls to form
            this.Controls.Add(avatarBox);
            this.Controls.Add(changeAvatarButton);
            this.Controls.Add(nameLabel);
            this.Controls.Add(nameBox);
            this.Controls.Add(usernameLabel);
            this.Controls.Add(usernameBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);
        }

        private async void LoadUserData()
        {
            try
            {
                // Load current user data
                nameBox.Text = AuctionClient.gI().Name;
                usernameBox.Text = AuctionClient.gI().Username;
                currentAvatarUrl = AuctionClient.gI().avatar_url;

                if (!string.IsNullOrEmpty(currentAvatarUrl))
                {
                    Image userImage = await Utils.Utils.LoadUserPicture(currentAvatarUrl);
                    if (userImage != null)
                    {
                        avatarBox.Image = userImage;
                    }
                }
                else
                {
                    // Load default avatar
                    Image defaultAvatar = await Utils.Utils.LoadUserPicture("https://www.w3schools.com/howto/img_avatar.png");
                    if (defaultAvatar != null)
                    {
                        avatarBox.Image = defaultAvatar;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin người dùng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeAvatarButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Handle cross-thread operation properly
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => ChangeAvatarButton_Click(sender, e)));
                    return;
                }

                // Create and configure the thread for opening the file dialog
                Thread openFileThread = new Thread(OpenFileDialogThread);
                openFileThread.SetApartmentState(ApartmentState.STA);
                openFileThread.Start();
                openFileThread.Join(); // Wait for thread to complete
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenFileDialogThread()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                    openFileDialog.Title = "Chọn ảnh đại diện";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        selectedImagePath = filePath;

                        // Update UI on the UI thread
                        this.BeginInvoke(new Action(() =>
                        {
                            LoadSelectedImage(filePath);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi chọn ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSelectedImage(string filePath)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => LoadSelectedImage(filePath)));
                return;
            }

            try
            {
                // Safely dispose of old image
                SafeDisposeImage(avatarBox.Image);

                // Create a new image from file
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var newImage = Image.FromStream(stream);
                    avatarBox.Image = new Bitmap(newImage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SafeDisposeImage(Image image)
        {
            if (image != null)
            {
                try
                {
                    if (avatarBox.InvokeRequired)
                    {
                        avatarBox.Invoke(new Action(() =>
                        {
                            avatarBox.Image = null;
                            image.Dispose();
                        }));
                    }
                    else
                    {
                        avatarBox.Image = null;
                        image.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi giải phóng ảnh: {ex.Message}");
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ và tên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create update profile message
                Message msg = new Message(CommandType.UpdateProfile);
                msg.WriteUTF(nameBox.Text);
                msg.WriteUTF(usernameBox.Text);
                msg.WriteUTF(passwordBox.Text);

                // Nếu có ảnh mới được chọn, gửi file ảnh
                if (!string.IsNullOrEmpty(selectedImagePath))
                {
                    msg.WriteFile(selectedImagePath);
                }
                else
                {
                    // Nếu không có ảnh mới, gửi URL ảnh hiện tại
                    msg.WriteUTF(currentAvatarUrl);
                }

                AuctionClient.SendMessage(msg);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật thông tin: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void HandleUpdateProfileResponse(Message message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleUpdateProfileResponse(message)));
                return;
            }

            try
            {
                bool success = message.ReadBoolean();
                if (success)
                {
                    string name = message.ReadUTF();
                    string avatar_url = message.ReadUTF();

                    // Cập nhật thông tin người dùng
                    AuctionClient.gI().Name = name;
                    if (!string.IsNullOrEmpty(avatar_url))
                    {
                        AuctionClient.gI().avatar_url = avatar_url;
                    }

                    FormLobby.gI().LoadAndRenderUserPicture(avatar_url);

                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    string errorMessage = message.ReadUTF();
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý phản hồi cập nhật profile: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}