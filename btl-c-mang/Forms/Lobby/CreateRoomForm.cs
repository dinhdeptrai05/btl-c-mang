using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Client.Core;
using Client.enums;
using Client.Model;
using Message = Client.Core.Message;

namespace Client.Forms.Lobby
{
    public partial class CreateRoomForm : Form
    {
        private List<Item> items = new List<Item>();
        private int minParticipants = 5;
        private ListView itemsListView;
        private TextBox roomNameBox;
        private NumericUpDown minParticipantsBox;
        private Label itemCountLabel;

        public CreateRoomForm()
        {
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Form settings
            this.Text = "Tạo phòng đấu giá mới";
            this.Size = new Size(1000, 600); // Tăng kích thước form để hiển thị đầy đủ thông tin
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(35, 35, 50);

            // Room info panel
            Panel roomInfoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(45, 45, 65)
            };

            Label roomNameLabel = new Label
            {
                Text = "Tên phòng:",
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            roomNameBox = new TextBox
            {
                Location = new Point(150, 20),
                Width = 300,
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };

            Label minParticipantsLabel = new Label
            {
                Text = "Số người tối thiểu:",
                ForeColor = Color.White,
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            minParticipantsBox = new NumericUpDown
            {
                Location = new Point(150, 60),
                Width = 100,
                Minimum = 2,
                Maximum = 20,
                Value = minParticipants,
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10)
            };

            itemCountLabel = new Label
            {
                Text = "Số vật phẩm: 0",
                ForeColor = Color.Yellow,
                Location = new Point(500, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            // Panel chứa nút thêm vật phẩm và tạo phòng
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(40, 40, 60)
            };

            Button addItemButton = new Button
            {
                Text = "➕ Thêm vật phẩm",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 10),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Button createRoomButton = new Button
            {
                Text = "✅ Tạo phòng",
                BackColor = Color.FromArgb(70, 180, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(700, 10),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Panel chính cho danh sách vật phẩm
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(35, 35, 50)
            };

            // Khởi tạo ListView với các cột
            itemsListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            // Thêm các cột cho ListView với kích thước phù hợp
            itemsListView.Columns.Add("Tên vật phẩm", 200);
            itemsListView.Columns.Add("Mô tả", 290);
            itemsListView.Columns.Add("Giá khởi điểm", 130);
            itemsListView.Columns.Add("Giá mua ngay", 130);
            itemsListView.Columns.Add("Thời gian kết thúc", 190);

            // Add controls to panels
            roomInfoPanel.Controls.AddRange(new Control[] {
                roomNameLabel, roomNameBox,
                minParticipantsLabel, minParticipantsBox,
                itemCountLabel
            });

            buttonPanel.Controls.AddRange(new Control[] {
                addItemButton, createRoomButton
            });

            mainPanel.Controls.Add(itemsListView);

            // Add panels to form
            this.Controls.Add(mainPanel);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(roomInfoPanel);

            // Event handlers
            addItemButton.Click += (s, e) =>
            {
                using (AddItemForm addItemForm = new AddItemForm())
                {
                    if (addItemForm.ShowDialog() == DialogResult.OK)
                    {
                        Item newItem = addItemForm.GetItem();
                        items.Add(newItem);

                        // Cập nhật ListView với tất cả vật phẩm
                        RefreshItemsList();
                    }
                }
            };

            createRoomButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(roomNameBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên phòng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (items.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một vật phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Gửi thông tin phòng và vật phẩm lên server
                Message msg = new Message(CommandType.CreateRoom);
                msg.WriteUTF(roomNameBox.Text);
                msg.WriteInt((int)minParticipantsBox.Value);
                msg.WriteInt((int)AuctionClient.gI().UserId);
                msg.WriteInt(items.Count);

                foreach (Item item in items)
                {
                    msg.WriteInt(item.Id);
                    msg.WriteUTF(item.Name);
                    msg.WriteUTF(item.Description);
                    msg.WriteUTF(item.ImageURL);
                    msg.WriteDouble(item.StartingPrice);
                    msg.WriteDouble(item.BuyNowPrice);
                    msg.WriteUTF(item.EndTime.ToString("HH:mm:ss dd/MM/yyyy"));
                }

                AuctionClient.SendMessage(msg);
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
        }

        // Phương thức mới để cập nhật danh sách vật phẩm
        private void RefreshItemsList()
        {
            itemsListView.BeginUpdate();
            itemsListView.Items.Clear();

            foreach (Item it in items)
            {
                ListViewItem lvi = new ListViewItem(it.Name);
                lvi.SubItems.Add(it.Description);
                lvi.SubItems.Add(it.StartingPrice.ToString("N0") + " VND");
                lvi.SubItems.Add(it.BuyNowPrice.ToString("N0") + " VND");
                lvi.SubItems.Add(it.EndTime.ToString("dd/MM/yyyy HH:mm:ss"));
                itemsListView.Items.Add(lvi);
            }

            itemsListView.EndUpdate();
            itemCountLabel.Text = $"Số vật phẩm: {items.Count}";
        }
    }
}