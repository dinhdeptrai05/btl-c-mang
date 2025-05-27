using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Client.Forms.Lobby;
using Client.Model;
using Message = Client.Core.Message;

namespace Client.Forms
{
    public partial class FormLobby : Form
    {
        private int secondsLeft = 0;
        private static FormLobby instance;
        public List<Room> rooms = new List<Room>();
        private Room currentRoom; // Lưu trữ phòng hiện tại để truy cập nhanh
        private DateTime lastUpdateTime;
        private const int TIMER_INTERVAL = 100; // Update every 100ms for smoother countdown

        public static FormLobby gI()
        {
            return instance;
        }

        public FormLobby()
        {
            instance = this;
            InitializeComponent();
            InitializeAuctionTimer();
            StartTimers();
            CustomizeDesign();

            // Đăng ký handler nhận danh sách phòng và phản hồi tham gia phòng
            AuctionClient.gI().RegisterHandler(CommandType.getAllRoomResponse, HandleLoadRoomsResponse);
            AuctionClient.gI().RegisterHandler(CommandType.JoinRoomResponse, HandleJoinRoomResponse);
            AuctionClient.gI().RegisterHandler(CommandType.CreateRoomResponse, HandleCreateRoomResponse);
            AuctionClient.gI().RegisterHandler(CommandType.AuctionStarted, HandleAuctionStarted);
            AuctionClient.gI().RegisterHandler(CommandType.BuyNowResponse, HandleBuyNowResponse);
            AuctionClient.gI().RegisterHandler(CommandType.AuctionEnd, HandleAuctionEnd);
            AuctionClient.gI().RegisterHandler(CommandType.RoomClosed, HandleRoomClosed);
            AuctionClient.gI().RegisterHandler(CommandType.KickedFromRoom, HandleKickedFromRoom);


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

            // Thêm sự kiện đóng form
            this.FormClosing += FormLobby_FormClosing;
        }

        private void InitializeAuctionTimer()
        {
            auctionTimer = new System.Windows.Forms.Timer();
            auctionTimer.Interval = TIMER_INTERVAL;
            auctionTimer.Tick += AuctionTimer_Tick;
            lastUpdateTime = DateTime.Now;
        }

        private void StartTimers()
        {
            clockTimer.Start();
            auctionTimer.Start();
        }


        // Thêm vào constructor của FormLobby
        private void InitializeBidInput()
        {
            // Thiết lập cho NumericUpDown bidInput
            bidInput.DecimalPlaces = 0;
            bidInput.ThousandsSeparator = true;
            bidInput.Increment = 100000; // Tăng 100k mỗi lần
            bidInput.Minimum = 0;

            // Xử lý khi người dùng thay đổi giá trị
            bidInput.ValueChanged += BidInput_ValueChanged;
            bidInput.KeyPress += BidInput_KeyPress;
        }

        private void BidInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cho phép Enter để đặt giá nhanh
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                PlaceBid(GetCurrentItem());
            }
        }

        private void BidInput_ValueChanged(object sender, EventArgs e)
        {
            // Chỉ đảm bảo giá đặt cao hơn giá hiện tại ít nhất 100k
            Item currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                double currentPrice = currentItem.LastestBidPrice > currentItem.StartingPrice ? currentItem.LastestBidPrice : currentItem.StartingPrice;
                double minimumBid = currentPrice + 100000;

                if (bidInput.Value < (decimal)minimumBid)
                {
                    bidInput.Value = (decimal)minimumBid;
                }
            }
        }

        private void CustomizeDesign()
        {
            InitializeBidInput();
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

            LoadAndRenderUserPicture(userPictureUrl);
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

        private void AuctionTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (currentRoom == null || currentRoom.Items == null || currentRoom.Items.Count == 0)
                {
                    auctionTimer.Stop();
                    return;
                }

                Item currentItem = GetCurrentItem();
                if (currentItem == null || currentItem.isSold)
                {
                    auctionTimer.Stop();
                    return;
                }

                // Calculate elapsed time since last update
                var now = DateTime.Now;
                var elapsed = (now - lastUpdateTime).TotalMilliseconds;
                lastUpdateTime = now;

                // Update time left
                currentItem.TimeLeft -= (long)elapsed;
                if (currentItem.TimeLeft <= 0)
                {
                    currentItem.TimeLeft = 0;
                    currentItem.isSold = true;
                    auctionTimer.Stop();

                    // Update UI to show auction ended
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            timeRemainingLabel.Text = "Đấu giá đã kết thúc";
                            bidInput.Enabled = false;
                            placeBidButton.Enabled = false;
                        }));
                    }
                    else
                    {
                        timeRemainingLabel.Text = "Đấu giá đã kết thúc";
                        bidInput.Enabled = false;
                        placeBidButton.Enabled = false;
                    }
                    return;
                }

                // Update UI with remaining time
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        int minutes = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalMinutes;
                        int seconds = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalSeconds % 60;
                        timeRemainingLabel.Text = $"Thời gian còn lại: {minutes:00}:{seconds:00}";
                    }));
                }
                else
                {
                    int minutes = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalMinutes;
                    int seconds = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalSeconds % 60;
                    timeRemainingLabel.Text = $"Thời gian còn lại: {minutes:00}:{seconds:00}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong bộ đếm thời gian: {ex}");
                auctionTimer.Stop();
            }
        }

        public async Task LoadAndRenderUserPicture(string imageUrl)
        {
            try
            {
                // Clear old image first
                if (userPictureBox.Image != null)
                {
                    var oldImage = userPictureBox.Image;
                    userPictureBox.Image = null;
                    oldImage.Dispose();
                }

                Image userImage = await Utils.Utils.LoadUserPicture(imageUrl);
                if (userImage != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            userPictureBox.Image = userImage;
                            userPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                            GraphicsPath gp = new GraphicsPath();
                            gp.AddEllipse(0, 0, userPictureBox.Width - 1, userPictureBox.Height - 1);
                            userPictureBox.Region = new Region(gp);
                        }));
                    }
                    else
                    {
                        userPictureBox.Image = userImage;
                        userPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddEllipse(0, 0, userPictureBox.Width - 1, userPictureBox.Height - 1);
                        userPictureBox.Region = new Region(gp);
                    }
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
                // Clear old image first
                if (itemPictureBox.Image != null)
                {
                    var oldImage = itemPictureBox.Image;
                    itemPictureBox.Image = null;
                    oldImage.Dispose();
                }

                Image itemImage = await Utils.Utils.LoadUserPicture(imageUrl);
                if (itemImage != null)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            itemPictureBox.Image = itemImage;
                            itemPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        }));
                    }
                    else
                    {
                        itemPictureBox.Image = itemImage;
                        itemPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải ảnh vật phẩm: {ex}");
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        itemPictureBox.Image = null;
                    }));
                }
                else
                {
                    itemPictureBox.Image = null;
                }
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

        private void SwitchToAuctionInterface(Room room)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => SwitchToAuctionInterface(room)));
                    return;
                }

                // Clear old data first
                itemPictureBox.Image = null;
                itemNameLabel.Text = "";
                itemDescLabel.Text = "";
                currentPriceLabel.Text = "Giá hiện tại: N/A";
                lastBidderLabel.Text = "Người đấu giá: N/A";
                timeRemainingLabel.Text = "Thời gian còn lại: N/A";
                bidInput.Enabled = false;
                placeBidButton.Enabled = false;

                currentRoom = room;
                roomsPanel.Visible = false;
                auctionMainPanel.Visible = true;

                // Xóa nút Mua ngay cũ nếu có
                foreach (Control control in auctionInfoPanel.Controls.OfType<Button>().ToList())
                {
                    if (control.Text.StartsWith("Mua ngay"))
                    {
                        auctionInfoPanel.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                roomNameLabel.Text = $"Phòng đấu giá: {room.Name} (ID: {room.Id})";

                Item currentItem = GetCurrentItem();
                if (currentItem != null)
                {
                    itemNameLabel.Text = currentItem.Name ?? "Không xác định";
                    itemDescLabel.Text = currentItem.Description ?? "Không có mô tả";
                    currentPriceLabel.Text = $"Giá hiện tại: {(currentItem.LastestBidPrice > currentItem.StartingPrice ? currentItem.LastestBidPrice : currentItem.StartingPrice):N0} VNĐ";
                    lastBidderLabel.Text = $"Người đấu giá: {(string.IsNullOrEmpty(currentItem.LastestBidderName) ? "Chưa có" : currentItem.LastestBidderName)}";

                    // Tính thời gian còn lại
                    int minutes = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalMinutes;
                    int seconds = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalSeconds % 60;
                    timeRemainingLabel.Text = $"Thời gian còn lại: {minutes:00}:{seconds:00}";

                    // Thiết lập bidInput với logic cải thiện
                    SetupBidInputForItem(currentItem);

                    if (currentItem != null && currentItem.BuyNowPrice > 0 && !currentItem.isSold && currentItem.TimeLeft > 0)
                    {
                        Button buyNowButton = new Button
                        {
                            Text = $"Mua ngay ({currentItem.BuyNowPrice:N0} VNĐ)",
                            BackColor = Color.FromArgb(70, 130, 180),
                            ForeColor = Color.White,
                            FlatStyle = FlatStyle.Flat,
                            Location = new Point(placeBidButton.Location.X, placeBidButton.Location.Y + 45),
                            Size = new Size(120, 35)
                        };
                        buyNowButton.Click += (s, e) => BuyNow(currentItem);
                        auctionInfoPanel.Controls.Add(buyNowButton);
                    }

                    LoadAndRenderItemPicture(currentItem.ImageURL ?? "https://via.placeholder.com/360x260");
                }

                // Start the timer when switching to auction interface
                lastUpdateTime = DateTime.Now;
                auctionTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi chuyển đổi giao diện đấu giá: {ex}");
                MessageBox.Show($"Lỗi khi chuyển đổi giao diện đấu giá: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Cập nhật HandlePlaceBidResponse để cập nhật bidInput khi có người khác đặt giá:
        public void HandlePlaceBidResponse(Message message)
        {
            try
            {
                // Đọc dữ liệu cơ bản
                int roomId = message.ReadInt();
                int itemId = message.ReadInt();
                double bidPrice = message.ReadDouble();
                long timeLeft = message.ReadLong();
                string bidderName = message.ReadUTF();

                if (currentRoom != null && currentRoom.Id == roomId)
                {
                    Item currentItem = currentRoom.Items.FirstOrDefault(i => i.Id == itemId);
                    if (currentItem != null)
                    {
                        // Cập nhật thông tin đấu giá
                        currentItem.LastestBidPrice = bidPrice;
                        currentItem.LastestBidderName = bidderName;
                        try
                        {
                            currentItem.TimeLeft = timeLeft;
                        }
                        catch
                        {
                            currentItem.TimeLeft = 10 * 60 * 1000;
                        }

                        // Cập nhật UI
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                currentPriceLabel.Text = $"Giá hiện tại: {bidPrice:N0} VNĐ";
                                lastBidderLabel.Text = $"Người đặt giá cuối: {bidderName}";

                                // Cập nhật bidInput với giá tối thiểu mới
                                double minimumBid = bidPrice + 100000;
                                bidInput.Minimum = (decimal)minimumBid;
                                if (bidInput.Value < (decimal)minimumBid)
                                {
                                    bidInput.Value = (decimal)minimumBid;
                                }

                                int minutes = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalMinutes;
                                int seconds = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalSeconds % 60;
                                timeRemainingLabel.Text = $"Thời gian còn lại: {minutes:00}:{seconds:00}";
                            }));
                        }
                        else
                        {
                            currentPriceLabel.Text = $"Giá hiện tại: {bidPrice:N0} VNĐ";
                            lastBidderLabel.Text = $"Người đặt giá cuối: {bidderName}";

                            // Cập nhật bidInput với giá tối thiểu mới
                            double minimumBid = bidPrice + 100000;
                            bidInput.Minimum = (decimal)minimumBid;
                            if (bidInput.Value < (decimal)minimumBid)
                            {
                                bidInput.Value = (decimal)minimumBid;
                            }

                            int minutes = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalMinutes;
                            int seconds = (int)TimeSpan.FromMilliseconds(currentItem.TimeLeft).TotalSeconds % 60;
                            timeRemainingLabel.Text = $"Thời gian còn lại: {minutes:00}:{seconds:00}";
                        }

                        // Thông báo trong chat
                        string formattedMessage = $"[{DateTime.Now:HH:mm:ss}] {bidderName} đã đặt giá {bidPrice:N0} VNĐ";
                        AddMessageToChat(formattedMessage, Color.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandlePlaceBidResponse: {ex.Message}");
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
                if (currentRoom != null && currentRoom.OwnerId == AuctionClient.gI().UserId)
                {
                    MessageBox.Show("Chủ phòng không thể mua vật phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    msg.WriteInt(currentRoom.Id);
                    msg.WriteInt(item.Id);
                    msg.WriteDouble(item.BuyNowPrice);
                    msg.WriteInt(AuctionClient.gI().UserId);
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
                                    formattedMessage = $"[{time}] Bạn: {chatMessage}";
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
                                string formattedMessage = $"HỆ THỐNG: {name} đã tham gia phòng!";

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
                                if (currentRoom != null)
                                {
                                    currentRoom.Chats.Add(new Chat(time, name, formattedMessage));
                                }
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
                currentRoom.Items.Clear();
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

                if (item.isSold)
                {
                    MessageBox.Show("Vật phẩm này đã được bán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (item.TimeLeft <= 0)
                {
                    MessageBox.Show("Thời gian đấu giá đã hết!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentRoom != null && currentRoom.OwnerId == AuctionClient.gI().UserId)
                {
                    MessageBox.Show("Chủ phòng không thể mua vật phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double currentPrice = item.LastestBidPrice > 0 ? item.LastestBidPrice : item.StartingPrice;
                double bidPrice = (double)bidInput.Value;

                // Kiểm tra giá tối thiểu (phải cao hơn giá hiện tại ít nhất 100k)
                double minimumBid = currentPrice + 100000;

                if (bidPrice < minimumBid)
                {
                    MessageBox.Show(
                        $"Giá đấu phải cao hơn giá hiện tại ít nhất 100,000 VNĐ!\n" +
                        $"Giá hiện tại: {currentPrice:N0} VNĐ\n" +
                        $"Giá tối thiểu: {minimumBid:N0} VNĐ",
                        "Giá đấu không hợp lệ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    // Tự động điều chỉnh giá về mức tối thiểu
                    bidInput.Value = (decimal)minimumBid;
                    return;
                }

                // Kiểm tra nếu giá đấu vượt quá giá mua ngay
                if (item.BuyNowPrice > 0 && bidPrice >= item.BuyNowPrice)
                {
                    var result = MessageBox.Show(
                        $"Giá đấu của bạn ({bidPrice:N0} VNĐ) bằng hoặc cao hơn giá mua ngay ({item.BuyNowPrice:N0} VNĐ).\n" +
                        $"Bạn có muốn mua ngay với giá {item.BuyNowPrice:N0} VNĐ không?",
                        "Mua ngay?",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        BuyNow(item);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                // Xác nhận đặt giá
                var confirmResult = MessageBox.Show(
                    $"Xác nhận đặt giá {bidPrice:N0} VNĐ cho vật phẩm '{item.Name}'?",
                    "Xác nhận đặt giá",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                // Gửi lệnh đặt giá đến server
                Message msg = new Message(CommandType.PlaceBid);
                msg.WriteInt(currentRoom.Id);
                msg.WriteInt(item.Id);
                msg.WriteDouble(bidPrice);
                msg.WriteInt(AuctionClient.gI().UserId);
                AuctionClient.SendMessage(msg);

                // Vô hiệu hóa tạm thời để tránh spam
                bidInput.Enabled = false;
                placeBidButton.Enabled = false;
                placeBidButton.Text = "Đang xử lý...";

                // Kích hoạt lại sau 2 giây
                Task.Delay(2000).ContinueWith(_ =>
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            bidInput.Enabled = true;
                            placeBidButton.Enabled = true;
                            placeBidButton.Text = "Đặt giá";
                        }));
                    }
                    else
                    {
                        bidInput.Enabled = true;
                        placeBidButton.Enabled = true;
                        placeBidButton.Text = "Đặt giá";
                    }
                });

                Console.WriteLine($"Đã gửi lệnh đặt giá: {bidPrice:N0} VNĐ cho vật phẩm ID {item.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đặt giá: {ex}");
                MessageBox.Show($"Lỗi khi đặt giá: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Đảm bảo kích hoạt lại controls nếu có lỗi
                bidInput.Enabled = true;
                placeBidButton.Enabled = true;
                placeBidButton.Text = "Đặt giá";
            }
        }

        // Cải thiện method SwitchToAuctionInterface để thiết lập bidInput tốt hơn
        private void SetupBidInputForItem(Item item)
        {
            if (item == null) return;

            double currentPrice = item.LastestBidPrice > 0 ? item.LastestBidPrice : item.StartingPrice;
            double minimumBid = currentPrice + 100000; // Tối thiểu tăng 100k

            bidInput.Minimum = (decimal)minimumBid;
            bidInput.Maximum = 99999999999999;
            bidInput.Value = (decimal)minimumBid;
            bidInput.Increment = 100000; // Tăng 100k mỗi lần click

            bidInput.Enabled = !item.isSold && item.TimeLeft > 0;
            placeBidButton.Enabled = !item.isSold && item.TimeLeft > 0;
        }

        // Thêm method để cập nhật nhanh giá đấu
        private void AddQuickBidButtons()
        {
            if (currentRoom == null || GetCurrentItem() == null) return;

            Item currentItem = GetCurrentItem();
            double currentPrice = currentItem.LastestBidPrice > 0 ? currentItem.LastestBidPrice : currentItem.StartingPrice;

            // Tạo các nút đặt giá nhanh
            int[] quickBidAmounts = { 100000, 500000, 1000000 }; // 100k, 500k, 1M

            for (int i = 0; i < quickBidAmounts.Length; i++)
            {
                Button quickBidBtn = new Button
                {
                    Text = $"+{quickBidAmounts[i] / 1000}K",
                    Size = new Size(60, 30),
                    Location = new Point(bidInput.Location.X + bidInput.Width + 10 + (i * 65), bidInput.Location.Y),
                    BackColor = Color.FromArgb(100, 149, 237),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold)
                };

                int amount = quickBidAmounts[i]; // Capture for closure
                quickBidBtn.Click += (s, e) =>
                {
                    bidInput.Value = Math.Min(bidInput.Maximum, bidInput.Value + amount);
                };

                // Thêm vào panel chứa bidInput (cần xác định panel này)
                // auctionInfoPanel.Controls.Add(quickBidBtn);
            }
        }

        private Item GetCurrentItem()
        {
            try
            {
                if (currentRoom?.Items == null) return null;

                return currentRoom.Items.FirstOrDefault(item =>
                    !item.isSold &&
                    item.TimeLeft > 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy vật phẩm hiện tại: {ex}");
                return null;
            }
        }

        private void joinRoom_Click(object sender, EventArgs e)
        {
            // Tạo form nhập ID phòng
            Form joinRoomForm = new Form
            {
                Text = "Tham gia phòng",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(45, 45, 65)
            };

            // Label hướng dẫn
            Label label = new Label
            {
                Text = "Nhập ID phòng bạn muốn tham gia:",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Textbox nhập ID
            TextBox idInput = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(340, 30),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Nút tham gia
            Button joinButton = new Button
            {
                Text = "Tham gia",
                Location = new Point(280, 100),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(80, 120, 190),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            // Nút hủy
            Button cancelButton = new Button
            {
                Text = "Hủy",
                Location = new Point(190, 100),
                Size = new Size(80, 35),
                BackColor = Color.FromArgb(80, 80, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };

            // Xử lý sự kiện click nút tham gia
            joinButton.Click += (s, ev) =>
            {
                if (int.TryParse(idInput.Text, out int roomId))
                {
                    // Tìm phòng trong danh sách
                    Room targetRoom = rooms.FirstOrDefault(r => r.Id == roomId);
                    if (targetRoom != null)
                    {
                        JoinRoom(targetRoom);
                        joinRoomForm.Close();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy phòng với ID này!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng nhập ID phòng hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Xử lý sự kiện click nút hủy
            cancelButton.Click += (s, ev) => joinRoomForm.Close();

            // Thêm các control vào form
            joinRoomForm.Controls.Add(label);
            joinRoomForm.Controls.Add(idInput);
            joinRoomForm.Controls.Add(joinButton);
            joinRoomForm.Controls.Add(cancelButton);

            // Hiển thị form
            joinRoomForm.ShowDialog();
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
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                    }
                    else
                    {
                        MessageBox.Show(errorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                int roomId = message.ReadInt();
                string roomName = message.ReadUTF();
                int ownerId = message.ReadInt();
                string ownerName = message.ReadUTF();
                bool isOpen = message.ReadBoolean();
                int itemsNum = message.ReadInt();

                List<Item> items = new List<Item>();
                List<Chat> chats = new List<Chat>();
                for (int i = 0; i < itemsNum; i++)
                {
                    int itemId = message.ReadInt();
                    int lastBidderID = message.ReadInt();
                    string lastBidderName = message.ReadUTF();
                    string itemName = message.ReadUTF();
                    string itemDesc = message.ReadUTF();
                    string imageUrl = message.ReadUTF();
                    double startPrice = message.ReadDouble();
                    double buyNowPrice = message.ReadDouble();
                    double currentPrice = message.ReadDouble();
                    bool isSold = message.ReadBoolean();
                    long timeLeft = message.ReadLong();

                    // Tạo item mới với giá hiện tại (nếu có người đặt giá) hoặc giá khởi điểm
                    Item item = new Item(itemId, lastBidderID, itemName, itemDesc, imageUrl, startPrice, buyNowPrice,
                        currentPrice, isSold, timeLeft);
                    item.LastestBidderName = lastBidderName; // Cập nhật tên người đặt giá cuối
                    items.Add(item);
                }

                int chatsNum = message.ReadInt();
                for (int i = 0; i < chatsNum; i++)
                {
                    string time = message.ReadUTF();
                    string name = message.ReadUTF();
                    string msg = message.ReadUTF();
                    chats.Add(new Chat(time, name, msg));
                }

                Room joinedRoom = new Room(roomId, roomName, ownerId, ownerName, isOpen, items, chats);

                // Đảm bảo tất cả UI updates được thực hiện trên UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        SwitchToAuctionInterface(joinedRoom);
                        chatDisplayBox.Clear();
                        LoadChatHistory(chats);
                        ShowSystemMessage("Bạn đã tham gia phòng đấu giá!");
                    }));
                }
                else
                {
                    SwitchToAuctionInterface(joinedRoom);
                    chatDisplayBox.Clear();
                    LoadChatHistory(chats);
                    ShowSystemMessage("Bạn đã tham gia phòng đấu giá!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý phản hồi tham gia phòng: {ex}");
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => MessageBox.Show($"Lỗi khi tham gia phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
                else
                {
                    MessageBox.Show($"Lỗi khi tham gia phòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                        formattedMessage = $"[{chat.time}] Bạn: {chat.message}";
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
                    bool isStarted = message.ReadBoolean();
                    string owner_name = message.ReadUTF();
                    string name = message.ReadUTF();
                    string time_created = message.ReadUTF();

                    Room room = new Room(id, name, owner_id, owner_name, isOpen, isStarted);
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
                roomsPanel.SuspendLayout();

                roomsPanel.AutoScroll = true;
                roomsPanel.FlowDirection = FlowDirection.LeftToRight;
                roomsPanel.WrapContents = true;

                // Thêm padding để tạo khoảng cách ở đáy
                roomsPanel.Padding = new Padding(45, 0, 40, 0); // 50px khoảng cách ở đáy


                if (rooms.Count == 0)
                {
                    Label noRoomsLabel = new Label
                    {
                        Text = "Không có phòng nào. Hãy tạo phòng mới!",
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 16, FontStyle.Bold),
                        Padding = new Padding(25)
                    };
                    roomsPanel.Controls.Add(noRoomsLabel);
                }
                else
                {
                    // Kích thước cố định cho mỗi panel phòng
                    int roomWidth = 310;
                    int roomHeight = 210;

                    // Tạo và thêm các panel phòng vào FlowLayoutPanel
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        Room room = rooms[i];
                        Panel roomPanel = CreateRoomPanel(room);

                        // Thiết lập kích thước và margin
                        roomPanel.Size = new Size(roomWidth, roomHeight);
                        roomPanel.Margin = new Padding(15, 20, 15, 20); // Tăng margin giữa các phòng

                        roomsPanel.Controls.Add(roomPanel);
                    }

                    Panel spacerPanel = new Panel
                    {
                        Size = new Size(1, 70),
                        Margin = new Padding(0)
                    };
                    roomsPanel.Controls.Add(spacerPanel);
                }

                roomsPanel.ResumeLayout();
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
                Width = 320, // Đồng bộ với kích thước trong DisplayRooms
                Height = 210,
                BackColor = room.isOpen ? Color.FromArgb(45, 45, 65) : Color.FromArgb(50, 50, 70),
                BorderStyle = BorderStyle.None
            };

            // Thêm hiệu ứng bo góc và đổ bóng
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

                    using (Pen pen = new Pen(Color.FromArgb(90, 90, 120), 1.5f))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            // Tạo một hiệu ứng nền gradient tùy thuộc vào trạng thái phòng
            panel.BackColor = room.isOpen
                ? Color.FromArgb(45, 45, 65)
                : Color.FromArgb(60, 50, 70);

            // Tiêu đề phòng với font lớn hơn và vị trí cân đối
            Label nameLabel = new Label
            {
                Text = room.Name,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = panel.Width - 40,
                Height = 35,
                Location = new Point(20, 15)
            };

            // Trạng thái phòng với viền bo tròn
            Label statusLabel = new Label
            {
                Text = room.isOpen ? "Đang mở" : "Đã đóng",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = room.isOpen ? Color.LightGreen : Color.LightCoral,
                BackColor = room.isOpen ? Color.FromArgb(30, 80, 30) : Color.FromArgb(80, 30, 30),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 100,
                Height = 25,
                Location = new Point((panel.Width - 100) / 2, 50)
            };
            // Bo tròn cho label trạng thái
            statusLabel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, statusLabel.Width, statusLabel.Height);
                    statusLabel.Region = new Region(path);
                }
            };

            // Thông tin ID
            Label idLabel = new Label
            {
                Text = $"ID: {room.Id}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = panel.Width - 40,
                Height = 22,
                Location = new Point(20, 85)
            };

            // Thông tin chủ phòng
            Label ownerLabel = new Label
            {
                Text = $"Chủ phòng: {room.OwnerName}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = panel.Width - 40,
                Height = 22,
                Location = new Point(20, 110)
            };

            // Thời gian tạo
            if (room.TimeCreated != null)
            {
                Label timeLabel = new Label
                {
                    Text = $"Thời gian tạo: {room.TimeCreated}",
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.Silver,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Width = panel.Width - 40,
                    Height = 22,
                    Location = new Point(20, 135)
                };
                panel.Controls.Add(timeLabel);
            }

            // Nút tham gia hiện đại
            Button joinButton = new Button
            {
                Text = "Tham gia",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(80, 120, 190),
                Width = panel.Width - 80,
                Height = 35,
                Location = new Point(40, 160),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            joinButton.FlatAppearance.BorderSize = 0;
            joinButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 140, 220);
            joinButton.Click += (sender, e) => JoinRoom(room);

            panel.Controls.Add(nameLabel);
            panel.Controls.Add(statusLabel);
            panel.Controls.Add(idLabel);
            panel.Controls.Add(ownerLabel);
            panel.Controls.Add(joinButton);

            // Thêm hiệu ứng hover
            panel.Cursor = Cursors.Hand;
            panel.MouseEnter += (s, e) =>
            {
                panel.BackColor = room.isOpen
                    ? Color.FromArgb(50, 50, 72)
                    : Color.FromArgb(65, 55, 75);
            };
            panel.MouseLeave += (s, e) =>
            {
                panel.BackColor = room.isOpen
                    ? Color.FromArgb(45, 45, 65)
                    : Color.FromArgb(60, 50, 70);
            };
            panel.Click += (sender, e) => JoinRoom(room);

            return panel;
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            // Prevent opening profile settings if user is in a room
            if (currentRoom != null)
            {
                MessageBox.Show(
                    "Bạn không thể thay đổi cài đặt khi đang trong phòng đấu giá!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => settingsButton_Click(sender, e)));
                return;
            }

            // Hide the room list panel
            roomsPanel.Visible = false;

            // Show the profile settings form modally
            using (FormProfile profileForm = new FormProfile())
            {
                if (profileForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Update user info on the main UI after profile changes
                    userNameLabel.Text = AuctionClient.gI().Name ?? "Người dùng";
                    if (!string.IsNullOrEmpty(AuctionClient.gI().avatar_url))
                    {
                        userPictureUrl = AuctionClient.gI().avatar_url;
                        LoadAndRenderUserPicture(userPictureUrl);
                    }
                }
            }

            // Show the room list panel again after closing the profile form
            roomsPanel.Visible = true;
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

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Gửi message Logout tới server
            Message msg = new Message(CommandType.Logout);
            AuctionClient.SendMessage(msg);
        }

        private void FormLobby_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Gửi message Logout tới server trước khi đóng form
            Message msg = new Message(CommandType.Logout);
            AuctionClient.SendMessage(msg);

            if (auctionTimer != null)
            {
                auctionTimer.Stop();
                auctionTimer.Dispose();
            }
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

        public void HandleAuctionEnd(Message message)
        {
            try
            {
                int roomId = message.ReadInt();
                if (currentRoom != null && currentRoom.Id == roomId)
                {
                    ShowSystemMessage("Phiên đấu giá đã kết thúc!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý thông báo kết thúc đấu giá: {ex}");
            }
        }

        public void HandleRoomClosed(Message message)
        {
            try
            {
                int roomId = message.ReadInt();
                if (currentRoom != null && currentRoom.Id == roomId)
                {
                    ShowSystemMessage("Phòng đã được đóng!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý thông báo đóng phòng: {ex}");
            }
        }

        public void HandleKickedFromRoom(Message message)
        {
            try
            {
                int roomId = message.ReadInt();
                if (currentRoom != null && currentRoom.Id == roomId)
                {
                    ShowSystemMessage("Bạn đã bị đuổi khỏi phòng!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý thông báo đuổi khỏi phòng: {ex}");
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

                int roomId = message.ReadInt();
                int itemId = message.ReadInt();
                double price = message.ReadDouble();
                string buyerName = message.ReadUTF();

                Room room = rooms.FirstOrDefault(r => r.Id == roomId);
                if (room == null)
                {
                    Console.WriteLine($"Không tìm thấy phòng với ID: {roomId}");
                    return;
                }
                Item item = room.Items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    item.isSold = true;
                    item.LastestBidderName = buyerName;
                    item.LastestBidPrice = price;
                }

                // Chỉ hiển thị thông báo nếu người dùng đang ở trong phòng này
                if (currentRoom != null && currentRoom.Id == roomId)
                {
                    ShowSystemMessage($"Vật phẩm đã được mua bởi {buyerName} với giá {price:N0} VND!");
                }
                // Chuyển sang vật phẩm tiếp theo
                Item nextItem = GetCurrentItem();
                if (nextItem != null)
                {
                    SetupBidInputForItem(nextItem);
                    LoadAndRenderItemPicture(nextItem.ImageURL ?? "https://via.placeholder.com/360x260");
                    itemNameLabel.Text = nextItem.Name ?? "Không xác định";
                    itemDescLabel.Text = nextItem.Description ?? "Không có mô tả";
                    currentPriceLabel.Text = $"Giá hiện tại: {(nextItem.LastestBidPrice > nextItem.StartingPrice ? nextItem.LastestBidPrice : nextItem.StartingPrice):N0} VNĐ";
                    lastBidderLabel.Text = $"Người đấu giá: {(string.IsNullOrEmpty(nextItem.LastestBidderName) ? "Chưa có" : nextItem.LastestBidderName)}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý phản hồi mua ngay: {ex}");
                MessageBox.Show($"Lỗi khi mua ngay: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void HandleAddItemResponse(Message message)
        {
            bool success = message.ReadBoolean();
            string responseMessage = message.ReadUTF();

            if (success)
            {
                MessageBox.Show("Thêm vật phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(responseMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void HandleStartAuctionResponse(Message message)
        {
            bool success = message.ReadBoolean();
            string responseMessage = message.ReadUTF();

            if (success)
            {
                MessageBox.Show("Bắt đầu đấu giá thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(responseMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}