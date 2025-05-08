using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Client.Model;
using Client.Forms.Lobby;
using Message = Client.Core.Message;

namespace Client.Forms
{
    public partial class FormLobby : Form
    {
        private int secondsLeft = 0;
        private static FormLobby instance;
        public List<Room> rooms = new List<Room>();
        private Room currentRoom; // Lưu trữ phòng hiện tại để truy cập nhanh

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

            // Đăng ký handler nhận danh sách phòng và phản hồi tham gia phòng
            AuctionClient.gI().RegisterHandler(CommandType.getAllRoomResponse, HandleLoadRoomsResponse);
            AuctionClient.gI().RegisterHandler(CommandType.JoinRoomResponse, HandleJoinRoomResponse);
            AuctionClient.gI().RegisterHandler(CommandType.CreateRoomResponse, HandleCreateRoomResponse);
            AuctionClient.gI().RegisterHandler(CommandType.AuctionStarted, HandleAuctionStarted);

            // Đăng ký handler nhận tin nhắn chat
            AuctionClient.gI().RegisterHandler(CommandType.ChatMessageReceived, HandleChatMessageReceived);

            // Tải danh sách phòng khi form được khởi tạo
            LoadRooms();

            // Gán sự kiện cho các nút
            placeBidButton.Click += (sender, e) => PlaceBid(GetCurrentItem());
            leaveRoomButton.Click += (sender, e) => LeaveRoom();

            // Gán sự kiện cho nút gửi tin nhắn chat và ô nhập chat
            sendMessageButton.Click += sendMessageButton_Click;
            chatInputBox.KeyPress += chatInputBox_KeyPress;
        }

        private void StartTimers()
        {
            clockTimer.Start();
            auctionTimer.Start();
        }

        private async void CustomizeDesign()
        {
            userNameLabel.Text = AuctionClient.gI().Name ?? "Người dùng";

            if (!string.IsNullOrEmpty(AuctionClient.gI().avatar_url))
            {
                userPictureUrl = AuctionClient.gI().avatar_url;
            }
            else
            {
                userPictureUrl = "https://www.w3schools.com/howto/img_avatar.png";
            }

            foreach (Control control in sidePanel.Controls)
            {
                if (control is Button button)
                {
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 45, 65);
                }
            }

            await LoadAndRenderUserPicture(userPictureUrl);
        }

        private async Task<bool> LoadRooms()
        {
            try
            {
                Console.WriteLine("Đang gửi yêu cầu lấy danh sách phòng...");
                Message msg = new Message(CommandType.getAllRoom);
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
            if (currentRoom != null)
            {
                Item currentItem = GetCurrentItem();

                if (currentItem != null && currentItem.EndTime > DateTime.Now)
                {
                    TimeSpan remainingTime = currentItem.EndTime - DateTime.Now;
                    int hours = remainingTime.Hours;
                    int minutes = remainingTime.Minutes;
                    int seconds = remainingTime.Seconds;

                    secondsLeft = seconds;

                    timeRemainingLabel.Text = $"Thời gian còn lại: {hours:00}:{minutes:00}:{seconds:00}";

                }
                else
                {
                    // Auction has ended
                    timeRemainingLabel.Text = "Thời gian còn lại: Hết giờ";
                    bidInput.Enabled = false;
                    placeBidButton.Enabled = false;
                }
            }
        }


        private async Task LoadAndRenderUserPicture(string imageUrl)
        {
            try
            {
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

        private async Task LoadAndRenderItemPicture(string imageUrl)
        {
            try
            {
                Image itemImage = await Utils.Utils.LoadUserPicture(imageUrl);
                if (itemImage != null)
                {
                    itemPictureBox.Image = itemImage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải ảnh vật phẩm: {ex}");
            }
        }

        private void dashboardButton_Click(object sender, EventArgs e)
        {
            using (CreateRoomForm createRoomForm = new CreateRoomForm())
            {
                if (createRoomForm.ShowDialog() == DialogResult.OK)
                {
                    // Form đã đóng sau khi tạo phòng thành công
                    LoadRooms(); // Tải lại danh sách phòng
                }
            }
        }

        private void userNameLabel_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click vào tên người dùng (có thể mở settings)
        }

        private void userPictureBox_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click vào ảnh đại diện (có thể mở settings)
        }

        private async void SwitchToAuctionInterface(Room room)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => SwitchToAuctionInterface(room)));
                    return;
                }

                currentRoom = room; // Lưu phòng hiện tại
                roomsPanel.Visible = false;
                auctionMainPanel.Visible = true;

                roomNameLabel.Text = $"Phòng đấu giá: {room.Name} (ID: {room.Id})";

                Item currentItem = GetCurrentItem();
                if (currentItem != null)
                {
                    itemNameLabel.Text = currentItem.Name ?? "Không xác định";
                    itemDescLabel.Text = currentItem.Description ?? "Không có mô tả";
                    currentPriceLabel.Text = $"Giá hiện tại: {currentItem.LastestBidPrice:N0} VND";
                    lastBidderLabel.Text = $"Người đấu giá: {(string.IsNullOrEmpty(currentItem.LastestBidderName) ? "Chưa có" : currentItem.LastestBidderName)}";
                    timeRemainingLabel.Text = $"Thời gian còn lại: {secondsLeft / 3600:00}:{(secondsLeft % 3600) / 60:00}:{secondsLeft % 60:00}";
                    bidInput.Minimum = (decimal)(currentItem.LastestBidPrice + 100000);
                    bidInput.Value = bidInput.Minimum;
                    bidInput.Enabled = true;
                    placeBidButton.Enabled = true;

                    // Thêm nút mua ngay
                    Button buyNowButton = new Button
                    {
                        Text = $"Mua ngay ({currentItem.BuyNowPrice:N0} VND)",
                        BackColor = Color.FromArgb(70, 130, 180),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Location = new Point(placeBidButton.Location.X, placeBidButton.Location.Y + 45),
                        Size = new Size(120, 35)
                    };
                    buyNowButton.Click += (s, e) => BuyNow(currentItem);
                    auctionInfoPanel.Controls.Add(buyNowButton);

                    await LoadAndRenderItemPicture(currentItem.ImageURL ?? "https://via.placeholder.com/360x260");
                }
                else
                {
                    itemNameLabel.Text = "Không có vật phẩm đang đấu giá";
                    itemDescLabel.Text = "";
                    itemPictureBox.Image = null;
                    currentPriceLabel.Text = "Giá hiện tại: N/A";
                    lastBidderLabel.Text = "Người đấu giá: N/A";
                    timeRemainingLabel.Text = "Thời gian còn lại: N/A";
                    bidInput.Enabled = false;
                    placeBidButton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi chuyển đổi giao diện đấu giá: {ex}");
                MessageBox.Show($"Lỗi khi chuyển đổi giao diện đấu giá: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuyNow(Item item)
        {
            try
            {
                if (item == null)
                {
                    MessageBox.Show("Không có vật phẩm nào để mua!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn mua vật phẩm này với giá {item.BuyNowPrice:N0} VND?",
                    "Xác nhận mua ngay",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Message msg = new Message(CommandType.BuyNow);
                    msg.WriteInt(item.Id);
                    AuctionClient.SendMessage(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi mua ngay: {ex}");
                MessageBox.Show($"Lỗi khi mua ngay: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void HandleChatMessageReceived(Message message)
        {
            sbyte cmd = message.ReadSByte();
            switch ((int)cmd)
            {
                case 0:
                    {
                        try
                        {
                            int roomId = message.ReadInt();
                            string time = message.ReadUTF();
                            string name = message.ReadUTF();
                            string chatMessage = message.ReadUTF();

                            // Only process if this is for the current room
                            if (currentRoom != null && currentRoom.Id == roomId)
                            {
                                // Format: [Time] Name: Message
                                string formattedMessage = $"[{time}] {name}: {chatMessage}";

                                // Determine message color based on sender
                                Color messageColor;
                                if (name == AuctionClient.gI().Name)
                                {
                                    messageColor = Color.LightBlue; // Current user's messages
                                }
                                else if (name.ToLower().Contains("hệ thống") || name.ToLower().Contains("system"))
                                {
                                    messageColor = Color.Yellow; // System messages
                                }
                                else
                                {
                                    messageColor = Color.White; // Other users' messages
                                }

                                // Add message to chat display
                                AddMessageToChat(formattedMessage, messageColor);

                                // Also add to the room's chat history
                                currentRoom.Chats.Add(new Chat(time, name, chatMessage));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi xử lý tin nhắn chat: {ex}");
                        }
                    }
                    break;
                case 1:
                    {
                        try
                        {
                            int roomId = message.ReadInt();
                            string time = message.ReadUTF();
                            string name = message.ReadUTF();

                            // Only process if this is for the current room
                            if (currentRoom != null && currentRoom.Id == roomId)
                            {
                                // Format: [Time] Name: Message
                                string formattedMessage = $"[HỆ THỐNG: {name} đã tham gia phòng!";

                                Color messageColor;
                                messageColor = Color.Yellow; // System messages
                                AddMessageToChat(formattedMessage, messageColor);
                                currentRoom.Chats.Add(new Chat(time, name, formattedMessage));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi xử lý tin nhắn chat: {ex}");
                        }
                    }
                    break;
                case -1:
                    {
                        try
                        {
                            int roomId = message.ReadInt();
                            string time = message.ReadUTF();
                            string name = message.ReadUTF();

                            // Only process if this is for the current room
                            if (currentRoom != null && currentRoom.Id == roomId)
                            {
                                // Format: [Time] Name: Message
                                string formattedMessage = $"HỆ THỐNG: {name} đã rời phòng!";

                                Color messageColor;
                                messageColor = Color.Red; // System messages
                                AddMessageToChat(formattedMessage, messageColor);
                                currentRoom.Chats.Add(new Chat(time, name, formattedMessage));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi khi xử lý tin nhắn chat: {ex}");
                        }
                    }
                    break;
            }
        }

        private void LeaveRoom()
        {
            try
            {
                if (currentRoom == null) return;

                Message msg = new Message(CommandType.LeaveRoom);
                msg.WriteInt(currentRoom.Id);
                AuctionClient.SendMessage(msg);

                auctionMainPanel.Visible = false;
                roomsPanel.Visible = true;
                LoadRooms();
                currentRoom = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi rời phòng: {ex}");
                MessageBox.Show($"Lỗi khi rời phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlaceBid(Item item)
        {
            try
            {
                if (item == null)
                {
                    MessageBox.Show("Không có vật phẩm nào để đặt giá!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (bidInput.Value <= (decimal)item.LastestBidPrice)
                {
                    MessageBox.Show($"Giá đấu phải lớn hơn giá hiện tại ({item.LastestBidPrice:N0} VND)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double bidPrice = (double)bidInput.Value;

                Message msg = new Message(CommandType.PlaceBid);
                msg.WriteInt(item.Id);
                msg.WriteDouble(bidPrice);
                AuctionClient.SendMessage(msg);

                MessageBox.Show("Đã gửi giá đấu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đặt giá: {ex}");
                MessageBox.Show($"Lỗi khi đặt giá: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Item GetCurrentItem()
        {
            try
            {
                if (currentRoom?.Items == null) return null;

                return currentRoom.Items.FirstOrDefault(item =>
                    !item.isSold &&
                    item.EndTime > DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy vật phẩm hiện tại: {ex}");
                return null;
            }
        }

        private void JoinRoom(Room room)
        {
            try
            {
                if (!room.isOpen)
                {
                    MessageBox.Show("Phòng này đã đóng, bạn không thể tham gia!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Message msg = new Message(CommandType.JoinRoom);
                msg.WriteInt(room.Id);
                AuctionClient.SendMessage(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tham gia phòng: {ex}");
                MessageBox.Show($"Lỗi khi tham gia phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void HandleJoinRoomResponse(Message message)
        {
            try
            {
                bool success = message.ReadBoolean();
                if (!success)
                {
                    string errorMessage = message.ReadUTF();
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int roomId = message.ReadInt();
                string roomName = message.ReadUTF();
                int ownerId = message.ReadInt();
                bool isOpen = message.ReadBoolean();
                int itemsNum = message.ReadInt();

                // Đọc thời gian còn lại từ server (nếu có)
                //secondsLeft = message.ReadInt(); // Giả sử server gửi thời gian còn lại (thêm vào giao thức nếu cần)

                List<Item> items = new List<Item>();
                List<Chat> chats = new List<Chat>();
                for (int i = 0; i < itemsNum; i++)
                {
                    int itemId = message.ReadInt();
                    int lastBidderID = message.ReadInt();
                    string itemName = message.ReadUTF();
                    string itemDesc = message.ReadUTF();
                    string imageUrl = message.ReadUTF();
                    double startPrice = message.ReadDouble();
                    double buyNowPrice = message.ReadDouble();
                    double currentPrice = message.ReadDouble();
                    bool isSold = message.ReadBoolean();
                    string et = message.ReadUTF();
                    DateTime endTime = DateTime.Parse(et);

                    items.Add(new Item(itemId, lastBidderID, itemName, itemDesc, imageUrl, startPrice, buyNowPrice, currentPrice, isSold, endTime));
                }
                int chatsNum = message.ReadInt();
                for (int i = 0; i < chatsNum; i++)
                {
                    string time = message.ReadUTF();
                    string name = message.ReadUTF();
                    string msg = message.ReadUTF();

                    chats.Add(new Chat(time, name, msg));
                }

                Room joinedRoom = new Room(roomId, roomName, ownerId, isOpen, items, chats);

                // Use Invoke to ensure UI operations run on the UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        SwitchToAuctionInterface(joinedRoom);
                        // Clear chat display before loading chat history
                        chatDisplayBox.Clear();
                        LoadChatHistory(chats);
                        ShowSystemMessage("Bạn đã tham gia phòng đấu giá!");
                    }));
                }
                else
                {
                    SwitchToAuctionInterface(joinedRoom);
                    // Clear chat display before loading chat history
                    chatDisplayBox.Clear();
                    LoadChatHistory(chats);
                    ShowSystemMessage("Bạn đã tham gia phòng đấu giá!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý phản hồi tham gia phòng: {ex}");
                MessageBox.Show($"Lỗi khi tham gia phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChatHistory(List<Chat> chats)
        {
            try
            {
                if (chatDisplayBox.InvokeRequired)
                {
                    chatDisplayBox.Invoke(new Action<List<Chat>>(LoadChatHistory), chats);
                    return;
                }

                // First display a system message
                ShowSystemMessage("Lịch sử chat của phòng:");

                // Then display each chat message from the history
                foreach (Chat chat in chats)
                {
                    // Format: [Time] Name: Message
                    string formattedMessage = $"[{chat.time}] {chat.name}: {chat.message}";

                    // Add the message with appropriate color (you can customize this)
                    chatDisplayBox.SelectionStart = chatDisplayBox.TextLength;
                    chatDisplayBox.SelectionLength = 0;

                    // Use different colors for different users if desired
                    if (chat.name == AuctionClient.gI().Name)
                    {
                        chatDisplayBox.SelectionColor = Color.LightBlue; // Current user's messages
                    }
                    else if (chat.name.ToLower().Contains("hệ thống") || chat.name.ToLower().Contains("system"))
                    {
                        chatDisplayBox.SelectionColor = Color.Yellow; // System messages
                    }
                    else
                    {
                        chatDisplayBox.SelectionColor = Color.White; // Other users' messages
                    }

                    chatDisplayBox.AppendText(formattedMessage + Environment.NewLine);
                }

                // Scroll to the bottom to show the most recent messages
                chatDisplayBox.ScrollToCaret();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải lịch sử chat: {ex}");
                ShowSystemMessage($"Không thể tải lịch sử chat: {ex.Message}");
            }
        }

        public void HandleLoadRoomsResponse(Message message)
        {
            try
            {
                rooms.Clear();
                int roomCount = message.ReadInt();

                for (int i = 0; i < roomCount; i++)
                {
                    int id = message.ReadInt();
                    int owner_id = message.ReadInt();
                    bool isOpen = message.ReadBoolean();
                    string name = message.ReadUTF();
                    string time_created = message.ReadUTF();

                    Room room = new Room(id, name, owner_id, isOpen);
                    room.TimeCreated = time_created;
                    rooms.Add(room);
                }

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(DisplayRooms));
                }
                else
                {
                    DisplayRooms();
                }
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
                roomsPanel.Controls.Clear();

                foreach (Room room in rooms)
                {
                    Panel roomPanel = CreateRoomPanel(room);
                    roomsPanel.Controls.Add(roomPanel);
                }

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
                }

                roomsPanel.Refresh();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi hiển thị danh sách phòng: {ex}");
            }
        }

        private Panel CreateRoomPanel(Room room)
        {
            Panel panel = new Panel
            {
                Width = 220,
                Height = 170,
                Margin = new Padding(15),
                BackColor = Color.FromArgb(45, 45, 65),
                BorderStyle = BorderStyle.None
            };

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

            Label statusLabel = new Label
            {
                Text = room.isOpen ? "Đang mở" : "Đã đóng",
                Font = new Font("Segoe UI", 9),
                ForeColor = room.isOpen ? Color.LightGreen : Color.LightCoral,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panel.Width - 20,
                Height = 20,
                Location = new Point(10, 45)
            };

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

            panel.Controls.Add(nameLabel);
            panel.Controls.Add(statusLabel);
            panel.Controls.Add(idLabel);
            panel.Controls.Add(ownerLabel);
            panel.Controls.Add(joinButton);

            panel.Click += (sender, e) => JoinRoom(room);

            return panel;
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện mở settings
        }


        // Add these methods to the FormLobby class to handle chat functionality
        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        private void chatInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SendChatMessage();
            }
        }

        private void SendChatMessage()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(chatInputBox.Text) && currentRoom != null)
                {
                    string message = chatInputBox.Text.Trim();

                    // Clear the input box first
                    chatInputBox.Clear();

                    // Send the message to the server
                    Message msg = new Message(CommandType.SendChatMessage);
                    msg.WriteInt(currentRoom.Id); // Room ID
                    msg.WriteUTF((string)DateTime.Now.ToString("HH:mm:ss"));
                    msg.WriteUTF(AuctionClient.gI().Name);
                    msg.WriteUTF(message); // Chat message
                    AuctionClient.SendMessage(msg);

                    // Note: We don't add the message to the chat display here
                    // because we'll receive it back from the server with
                    // a proper timestamp, which will trigger the chat update handler
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi tin nhắn: {ex}");
                ShowSystemMessage($"Không thể gửi tin nhắn: {ex.Message}");
            }
        }

        public void AddMessageToChat(string message, Color color)
        {
            // Invoke required if called from non-UI thread
            if (chatDisplayBox.InvokeRequired)
            {
                chatDisplayBox.Invoke(new Action<string, Color>(AddMessageToChat), message, color);
                return;
            }

            // Add timestamp to messages
            string formattedMessage = $"{message}";

            // Add the message to the chat box
            chatDisplayBox.SelectionStart = chatDisplayBox.TextLength;
            chatDisplayBox.SelectionLength = 0;
            chatDisplayBox.SelectionColor = color;
            chatDisplayBox.AppendText(formattedMessage + Environment.NewLine);
            chatDisplayBox.ScrollToCaret();
        }

        // Example methods to show system messages when users join or leave
        public void ShowUserJoined(string username)
        {
            AddMessageToChat($"Người dùng {username} đã tham gia phòng", Color.LightGreen);
        }

        public void ShowUserLeft(string username)
        {
            AddMessageToChat($"Người dùng {username} đã rời phòng", Color.LightSalmon);
        }

        public void ShowSystemMessage(string message)
        {
            AddMessageToChat($"HỆ THỐNG: {message}", Color.Yellow);
        }

        // Call this when the Leave Room button is clicked
        private void leaveRoomButton_Click(object sender, EventArgs e)
        {
            // Notify the server that you're leaving
            // ...

            // Show rooms panel and hide auction panel
            auctionMainPanel.Visible = false;
            roomsPanel.Visible = true;
        }

        public void HandleCreateRoomResponse(Message message)
        {
            try
            {
                bool success = message.ReadBoolean();
                if (!success)
                {
                    string errorMessage = message.ReadUTF();
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int roomId = message.ReadInt();
                MessageBox.Show($"Tạo phòng thành công! ID phòng: {roomId}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRooms(); // Tải lại danh sách phòng
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý phản hồi tạo phòng: {ex}");
                MessageBox.Show($"Lỗi khi tạo phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void HandleAuctionStarted(Message message)
        {
            try
            {
                int roomId = message.ReadInt();
                if (currentRoom != null && currentRoom.Id == roomId)
                {
                    ShowSystemMessage("Phiên đấu giá đã bắt đầu!");
                    // Cập nhật UI nếu cần
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý thông báo bắt đầu đấu giá: {ex}");
            }
        }

        public void HandleBuyNowResponse(Message message)
        {
            try
            {
                bool success = message.ReadBoolean();
                if (!success)
                {
                    string errorMessage = message.ReadUTF();
                    MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int itemId = message.ReadInt();
                string buyerName = message.ReadUTF();
                double price = message.ReadDouble();

                if (currentRoom != null)
                {
                    ShowSystemMessage($"Vật phẩm đã được mua bởi {buyerName} với giá {price:N0} VND!");
                    // Cập nhật UI nếu cần
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý phản hồi mua ngay: {ex}");
                MessageBox.Show($"Lỗi khi mua ngay: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}