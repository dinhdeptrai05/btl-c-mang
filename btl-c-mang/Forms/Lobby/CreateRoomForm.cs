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

        public CreateRoomForm()
        {
            // InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Form settings
            this.Text = "Tạo phòng đấu giá mới";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(35, 35, 50);

            // Room info panel
            Panel roomInfoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                Padding = new Padding(20)
            };

            Label roomNameLabel = new Label
            {
                Text = "Tên phòng:",
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            TextBox roomNameBox = new TextBox
            {
                Location = new Point(120, 20),
                Width = 300,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label minParticipantsLabel = new Label
            {
                Text = "Số người tối thiểu:",
                ForeColor = Color.White,
                Location = new Point(20, 60),
                AutoSize = true
            };

            NumericUpDown minParticipantsBox = new NumericUpDown
            {
                Location = new Point(120, 60),
                Width = 100,
                Minimum = 2,
                Maximum = 20,
                Value = minParticipants,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Items panel
            Panel itemsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            ListView itemsListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                Dock = DockStyle.Fill
            };

            itemsListView.Columns.Add("Tên", 200);
            itemsListView.Columns.Add("Giá khởi điểm", 150);
            itemsListView.Columns.Add("Giá mua ngay", 150);
            itemsListView.Columns.Add("Thời gian kết thúc", 200);

            Button addItemButton = new Button
            {
                Text = "Thêm vật phẩm",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Bottom,
                Height = 40
            };

            Button createRoomButton = new Button
            {
                Text = "Tạo phòng",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Bottom,
                Height = 40
            };

            // Add controls to panels
            roomInfoPanel.Controls.AddRange(new Control[] { roomNameLabel, roomNameBox, minParticipantsLabel, minParticipantsBox });
            itemsPanel.Controls.Add(itemsListView);
            itemsPanel.Controls.Add(addItemButton);
            itemsPanel.Controls.Add(createRoomButton);

            // Add panels to form
            this.Controls.Add(roomInfoPanel);
            this.Controls.Add(itemsPanel);

            // Event handlers
            addItemButton.Click += (s, e) =>
            {
                using (AddItemForm addItemForm = new AddItemForm())
                {
                    if (addItemForm.ShowDialog() == DialogResult.OK)
                    {
                        Item newItem = addItemForm.GetItem();
                        items.Add(newItem);
                        itemsListView.Items.Add(new ListViewItem(new string[]
                        {
                            newItem.Name,
                            newItem.StartingPrice.ToString("N0") + " VND",
                            newItem.BuyNowPrice.ToString("N0") + " VND",
                            newItem.EndTime.ToString("dd/MM/yyyy HH:mm:ss")
                        }));
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
    }
}