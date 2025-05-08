using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Model;

namespace Client.Forms.Lobby
{
    public partial class AddItemForm : Form
    {
        private Item newItem;

        public AddItemForm()
        {
            // InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Form settings
            this.Text = "Thêm vật phẩm mới";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(35, 35, 50);

            // Main panel
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Item info controls
            Label nameLabel = new Label
            {
                Text = "Tên vật phẩm:",
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            TextBox nameBox = new TextBox
            {
                Location = new Point(120, 20),
                Width = 300,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label descLabel = new Label
            {
                Text = "Mô tả:",
                ForeColor = Color.White,
                Location = new Point(20, 60),
                AutoSize = true
            };

            TextBox descBox = new TextBox
            {
                Location = new Point(120, 60),
                Width = 300,
                Height = 100,
                Multiline = true,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label startPriceLabel = new Label
            {
                Text = "Giá khởi điểm:",
                ForeColor = Color.White,
                Location = new Point(20, 180),
                AutoSize = true
            };

            NumericUpDown startPriceBox = new NumericUpDown
            {
                Location = new Point(120, 180),
                Width = 200,
                Minimum = 100000,
                Maximum = 1000000000,
                Increment = 100000,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label buyNowPriceLabel = new Label
            {
                Text = "Giá mua ngay:",
                ForeColor = Color.White,
                Location = new Point(20, 220),
                AutoSize = true
            };

            NumericUpDown buyNowPriceBox = new NumericUpDown
            {
                Location = new Point(120, 220),
                Width = 200,
                Minimum = 100000,
                Maximum = 1000000000,
                Increment = 100000,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label endTimeLabel = new Label
            {
                Text = "Thời gian kết thúc:",
                ForeColor = Color.White,
                Location = new Point(20, 260),
                AutoSize = true
            };

            DateTimePicker endTimePicker = new DateTimePicker
            {
                Location = new Point(120, 260),
                Width = 200,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy HH:mm:ss",
                ShowUpDown = true,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White
            };

            Label imageUrlLabel = new Label
            {
                Text = "URL ảnh:",
                ForeColor = Color.White,
                Location = new Point(20, 300),
                AutoSize = true
            };

            TextBox imageUrlBox = new TextBox
            {
                Location = new Point(120, 300),
                Width = 300,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Button addButton = new Button
            {
                Text = "Thêm",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(120, 340),
                Width = 100,
                Height = 35
            };

            Button cancelButton = new Button
            {
                Text = "Hủy",
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(230, 340),
                Width = 100,
                Height = 35
            };

            // Add controls to panel
            mainPanel.Controls.AddRange(new Control[]
            {
                nameLabel, nameBox,
                descLabel, descBox,
                startPriceLabel, startPriceBox,
                buyNowPriceLabel, buyNowPriceBox,
                endTimeLabel, endTimePicker,
                imageUrlLabel, imageUrlBox,
                addButton, cancelButton
            });

            // Add panel to form
            this.Controls.Add(mainPanel);

            // Event handlers
            addButton.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên vật phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(descBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập mô tả vật phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (buyNowPriceBox.Value <= startPriceBox.Value)
                {
                    MessageBox.Show("Giá mua ngay phải lớn hơn giá khởi điểm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (endTimePicker.Value <= DateTime.Now)
                {
                    MessageBox.Show("Thời gian kết thúc phải lớn hơn thời gian hiện tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo vật phẩm mới
                newItem = new Item(
                    0, // ID sẽ được server gán
                    0, // LastestBidderId
                    nameBox.Text,
                    descBox.Text,
                    imageUrlBox.Text,
                    (double)startPriceBox.Value,
                    (double)buyNowPriceBox.Value,
                    (double)startPriceBox.Value, // LastestBidPrice ban đầu bằng giá khởi điểm
                    false, // isSold
                    endTimePicker.Value
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
        }

        public Item GetItem()
        {
            return newItem;
        }
    }
}