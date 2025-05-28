using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private DateTime startTime = DateTime.Now.AddMinutes(5);
        private ListView itemsListView;
        private TextBox roomNameBox;
        private DateTimePicker startTimePicker;
        private Label itemCountLabel;

        public CreateRoomForm()
        {
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Form settings
            this.Text = "Tạo phòng đấu giá mới";
            this.Size = new Size(1000, 600);
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

            Label startTimeLabel = new Label
            {
                Text = "Thời gian bắt đầu:",
                ForeColor = Color.White,
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            startTimePicker = new DateTimePicker
            {
                Location = new Point(150, 60),
                Width = 300,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "HH:mm dd/MM/yyyy",
                MinDate = DateTime.Now,
                Value = startTime,
                BackColor = Color.FromArgb(55, 55, 75),
                ForeColor = Color.White,
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
            itemsListView.Columns.Add("Ảnh", 150);
            itemsListView.Columns.Add("Tên vật phẩm", 210);
            itemsListView.Columns.Add("Mô tả", 320);
            itemsListView.Columns.Add("Giá khởi điểm", 140);
            itemsListView.Columns.Add("Giá mua ngay", 140);

            // Add controls to panels
            roomInfoPanel.Controls.AddRange(new Control[] {
                roomNameLabel, roomNameBox,
                startTimeLabel, startTimePicker,
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
                msg.WriteUTF(startTimePicker.Value.ToString("HH:mm:ss dd/MM/yyyy"));
                msg.WriteInt(AuctionClient.gI().UserId);
                msg.WriteInt(items.Count);

                foreach (Item item in items)
                {
                    msg.WriteUTF(item.Name);
                    msg.WriteUTF(item.Description);

                    string imagePath = Path.Combine(Application.StartupPath, "uploads", item.ImageURL);
                    if (File.Exists(imagePath))
                    {
                        msg.WriteFile(imagePath);
                    }
                    else
                    {
                        MessageBox.Show($"Không tìm thấy file ảnh: {imagePath}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    msg.WriteDouble(item.StartingPrice);
                    msg.WriteDouble(item.BuyNowPrice);
                    msg.WriteLong(item.TimeLeft);
                }

                AuctionClient.SendMessage(msg);
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            // Add context menu for edit/delete
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem editItem = new ToolStripMenuItem("Sửa");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Xóa");
            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(deleteItem);
            itemsListView.ContextMenuStrip = contextMenu;

            editItem.Click += (s, e) =>
            {
                if (itemsListView.SelectedItems.Count > 0)
                {
                    int index = itemsListView.SelectedIndices[0];
                    Item selectedItem = items[index];
                    using (AddItemForm editForm = new AddItemForm(selectedItem))
                    {
                        if (editForm.ShowDialog() == DialogResult.OK)
                        {
                            items[index] = editForm.GetItem();
                            RefreshItemsList();
                        }
                    }
                }
            };

            deleteItem.Click += (s, e) =>
            {
                if (itemsListView.SelectedItems.Count > 0)
                {
                    int index = itemsListView.SelectedIndices[0];
                    if (MessageBox.Show("Bạn có chắc chắn muốn xóa vật phẩm này?", "Xác nhận",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        items.RemoveAt(index);
                        RefreshItemsList();
                    }
                }
            };
        }

        private void RefreshItemsList()
        {
            itemsListView.BeginUpdate();
            itemsListView.Items.Clear();

            // Tạo ImageList mới cho mỗi lần refresh
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(80, 80); // Kích thước thumbnail
            itemsListView.SmallImageList = imageList;

            foreach (Item it in items)
            {
                ListViewItem lvi = new ListViewItem();

                // Thêm ảnh vào cột đầu tiên
                try
                {
                    string imagePath = Path.Combine(Application.StartupPath, "uploads", it.ImageURL);
                    if (File.Exists(imagePath))
                    {
                        using (Image img = Image.FromFile(imagePath))
                        {
                            // Tạo bản sao của ảnh với kích thước phù hợp
                            Bitmap thumbnail = new Bitmap(img, new Size(80, 80));
                            imageList.Images.Add(thumbnail);
                            lvi.ImageIndex = imageList.Images.Count - 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi tải ảnh: {ex.Message}");
                }

                // Thêm các thông tin khác
                lvi.SubItems.Add(it.Name);
                lvi.SubItems.Add(it.Description);
                lvi.SubItems.Add(it.StartingPrice.ToString("N0") + " VND");
                lvi.SubItems.Add(it.BuyNowPrice.ToString("N0") + " VND");
                itemsListView.Items.Add(lvi);
            }

            itemsListView.EndUpdate();
            itemCountLabel.Text = $"Số vật phẩm: {items.Count}";
        }
    }
}