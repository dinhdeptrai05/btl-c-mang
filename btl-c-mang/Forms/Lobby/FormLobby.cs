using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Client.Model;
using Message = Client.Core.Message;

namespace Client.Forms
{
    public partial class FormLobby : Form
    {
        // Timer for auction countdown
        private int secondsLeft = 3600; // 1 hour auction countdown

        private static FormLobby instance;

        public List<Room> rooms = new List<Room>();

        public static FormLobby gI()
        {
            return instance;
        }

        public FormLobby()
        {
            instance = this;
            InitializeComponent();
            StartTimers();
            CustomizeDesign();

            // Đăng ký handler nhận danh sách phòng
            AuctionClient.gI().RegisterHandler(CommandType.getAllRoomsResponse, HandleLoadRoomsResponse);

            // Tải danh sách phòng khi form được khởi tạo
            LoadRooms();
        }

        private void StartTimers()
        {
            // Start clock timer
            clockTimer.Start();

            // Start auction timer
            auctionTimer.Start();
        }

        private async void CustomizeDesign()
        {
            // Set user name from client
            userNameLabel.Text = AuctionClient.gI().Name;

            if (AuctionClient.gI().avatar_url != null && AuctionClient.gI().avatar_url != "")
            {
                userPictureUrl = AuctionClient.gI().avatar_url;
            }
            else
            {
                userPictureUrl = "https://www.w3schools.com/howto/img_avatar.png";
            }

            // Configure menu buttons hover effect
            foreach (Control control in sidePanel.Controls)
            {
                if (control is Button button)
                {
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 45, 65);
                }
            }

            // Add drawing for user picture (circle)
            LoadAndRenderUserPicture(userPictureUrl);
        }

        private async Task<bool> LoadRooms()
        {
            try
            {
                Console.WriteLine("Đang gửi yêu cầu lấy danh sách phòng...");
                Message msg = new Message(CommandType.getAllRooms);
                AuctionClient.SendMessage(msg);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách các phòng: {ex}");
                MessageBox.Show($"Lỗi khi lấy danh sách các phòng: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void clockTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            timeLabel.Text = now.ToString("HH:mm:ss");
            dateLabel.Text = now.ToString("dddd, MMMM d, yyyy");
        }

        private void auctionTimer_Tick(object sender, EventArgs e)
        {
            secondsLeft--;
            if (secondsLeft < 0)
            {
                secondsLeft = 3600; // Reset về 1 giờ
            }

            int hours = secondsLeft / 3600;
            int minutes = (secondsLeft % 3600) / 60;
            int seconds = secondsLeft % 60;

            // Cập nhật hiển thị thời gian đấu giá nếu có control
            if (Controls.Find("auctionTimerLabel", true).Length > 0)
            {
                Label auctionTimerLabel = (Label)Controls.Find("auctionTimerLabel", true)[0];
                auctionTimerLabel.Text = $"{hours:00}:{minutes:00}:{seconds:00}";
            }
        }

        private async void LoadAndRenderUserPicture(string imageUrl)
        {
            try
            {
                // Tải hình ảnh từ URL
                Image userImage = await Utils.Utils.LoadUserPicture(imageUrl);

                if (userImage != null)
                {
                    userPictureBox.Image = userImage;

                    userPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                    GraphicsPath gp = new GraphicsPath();
                    gp.AddEllipse(0, 0, userPictureBox.Width - 1, userPictureBox.Height - 1);
                    userPictureBox.Region = new Region(gp);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải ảnh đại diện: {ex}");
                MessageBox.Show($"Error rendering user picture: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dashboardButton_Click(object sender, EventArgs e)
        {
            // Làm mới danh sách phòng khi click vào dashboard
            LoadRooms();
        }

        private void userNameLabel_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click vào tên người dùng
        }

        private void userPictureBox_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click vào ảnh đại diện
        }

        public void HandleLoadRoomsResponse(Message message)
        {
            try
            {
                rooms.Clear(); // Xóa danh sách phòng cũ
                int roomCount = message.ReadInt();

                for (int i = 0; i < roomCount; i++)
                {
                    int id = message.ReadInt();
                    int owner_id = message.ReadInt();
                    int isOpen = message.ReadInt();
                    string name = message.ReadUTF();
                    string time_created = message.ReadUTF();

                    Room room = new Room(id, name, owner_id, isOpen);
                    room.TimeCreated = time_created;
                    rooms.Add(room);
                }

                // Cập nhật giao diện hiển thị danh sách phòng
                // Sử dụng Invoke để đảm bảo cập nhật UI trên thread chính
                this.Invoke(new Action(DisplayRooms));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý danh sách phòng: {ex}");
            }
        }

        private void DisplayRooms()
        {
            try
            {
                // Kiểm tra xem panel đã được tạo chưa
                if (roomsPanel == null)
                {
                    Console.WriteLine("Panel hiển thị phòng chưa được khởi tạo");
                    return;
                }

                // Xóa tất cả các phòng hiện tại trong panel
                roomsPanel.Controls.Clear();
                Console.WriteLine("Đã xóa các control cũ trong panel");

                // Thêm các phòng mới vào panel
                Console.WriteLine($"Đang hiển thị {rooms.Count} phòng");
                foreach (Room room in rooms)
                {
                    // Tạo một panel cho mỗi phòng
                    Panel roomPanel = CreateRoomPanel(room);
                    roomsPanel.Controls.Add(roomPanel);
                    Console.WriteLine($"Đã thêm phòng {room.Name} vào panel");
                }

                // Nếu không có phòng nào, hiển thị thông báo
                if (rooms.Count == 0)
                {
                    Label noRoomsLabel = new Label
                    {
                        Text = "Không có phòng nào. Hãy tạo phòng mới!",
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12),
                        Padding = new Padding(20)
                    };
                    roomsPanel.Controls.Add(noRoomsLabel);
                    Console.WriteLine("Không có phòng nào để hiển thị");
                }

                // Cập nhật panel
                roomsPanel.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi hiển thị danh sách phòng: {ex}");
            }
        }

        private Panel CreateRoomPanel(Room room)
        {
            // Tạo panel cho một phòng
            Panel panel = new Panel
            {
                Width = 220,
                Height = 170,
                Margin = new Padding(15),
                BackColor = Color.FromArgb(45, 45, 65),
                BorderStyle = BorderStyle.None
            };

            // Tạo hiệu ứng bo góc cho panel
            panel.Paint += (sender, e) =>
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 10;
                    Rectangle rect = new Rectangle(0, 0, panel.Width, panel.Height);
                    path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.X + rect.Width - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.X + rect.Width - radius * 2, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 90, 90);
                    path.CloseFigure();
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    panel.Region = new Region(path);
                }
            };

            // Tên phòng
            Label nameLabel = new Label
            {
                Text = room.Name,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panel.Width - 20,
                Height = 30,
                Location = new Point(10, 15)
            };

            // Trạng thái phòng
            Label statusLabel = new Label
            {
                Text = room.isOpen == 1 ? "Đang mở" : "Đã đóng",
                Font = new Font("Segoe UI", 9),
                ForeColor = room.isOpen == 1 ? Color.LightGreen : Color.LightCoral,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panel.Width - 20,
                Height = 20,
                Location = new Point(10, 45)
            };

            // ID phòng
            Label idLabel = new Label
            {
                Text = $"ID: {room.Id}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = panel.Width - 20,
                Height = 20,
                Location = new Point(10, 70)
            };

            // Chủ phòng
            Label ownerLabel = new Label
            {
                Text = $"Chủ phòng: {room.OwnerId}",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = panel.Width - 20,
                Height = 20,
                Location = new Point(10, 90)
            };

            // Thời gian tạo
            if (room.TimeCreated != null)
            {
                Label timeLabel = new Label
                {
                    Text = $"Thời gian tạo: {room.TimeCreated}",
                    Font = new Font("Segoe UI", 8),
                    ForeColor = Color.Silver,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Width = panel.Width - 20,
                    Height = 20,
                    Location = new Point(10, 110)
                };
                panel.Controls.Add(timeLabel);
            }

            // Nút tham gia phòng
            Button joinButton = new Button
            {
                Text = "Tham gia",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(80, 100, 170),
                Width = panel.Width - 60,
                Height = 30,
                Location = new Point(30, 135),
                Cursor = Cursors.Hand
            };

            joinButton.FlatAppearance.BorderSize = 0;
            joinButton.Click += (sender, e) => JoinRoom(room);

            // Thêm các control vào panel
            panel.Controls.Add(nameLabel);
            panel.Controls.Add(statusLabel);
            panel.Controls.Add(idLabel);
            panel.Controls.Add(ownerLabel);
            panel.Controls.Add(joinButton);

            // Sự kiện click vào panel
            panel.Click += (sender, e) => JoinRoom(room);

            return panel;
        }

        private void JoinRoom(Room room)
        {
            try
            {
                // Kiểm tra xem phòng có mở không
                if (room.isOpen != 1)
                {
                    MessageBox.Show("Phòng này đã đóng, bạn không thể tham gia!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Gửi yêu cầu tham gia phòng đến server
                //Message msg = new Message(CommandType.joinRoom);
                //msg.Writer.WriteInt(room.Id);
                //AuctionClient.SendMessage(msg);

                // Hiển thị thông báo đang xử lý
                MessageBox.Show($"Đang tham gia phòng {room.Name}...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tham gia phòng: {ex}");
                MessageBox.Show($"Lỗi khi tham gia phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}