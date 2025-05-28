using System;
using System.Drawing;
using System.Windows.Forms;
using Client.Model;
using System.IO;
using System.Threading;

namespace Client.Forms.Lobby
{
    public partial class AddItemForm : Form
    {
        private Item newItem;
        private string selectedImagePath;
        private PictureBox imagePreviewBox;
        private TextBox nameBox;
        private TextBox descBox;
        private NumericUpDown startPriceBox;
        private NumericUpDown buyNowPriceBox;

        public AddItemForm()
        {
            InitializeCustomComponents();
        }

        public AddItemForm(Item item)
        {
            InitializeCustomComponents();
            // Initialize form with existing item data
            nameBox.Text = item.Name;
            descBox.Text = item.Description;
            startPriceBox.Value = (decimal)item.StartingPrice;
            buyNowPriceBox.Value = (decimal)item.BuyNowPrice;
            selectedImagePath = Path.Combine(Application.StartupPath, "uploads", item.ImageURL);
            if (File.Exists(selectedImagePath))
            {
                LoadSelectedImage(selectedImagePath);
            }
            newItem = item; // Store the existing item
        }

        private void InitializeCustomComponents()
        {
            // Form settings
            this.Text = "Thêm vật phẩm mới";
            this.Size = new Size(500, 700);
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

            nameBox = new TextBox
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

            descBox = new TextBox
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

            startPriceBox = new NumericUpDown
            {
                Location = new Point(120, 180),
                Width = 200,
                Minimum = 100000,
                Maximum = 1000000000,
                Increment = 100000,
                Value = 100000,
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

            buyNowPriceBox = new NumericUpDown
            {
                Location = new Point(120, 220),
                Width = 200,
                Minimum = 100000,
                Maximum = 1000000000,
                Increment = 100000,
                Value = 200000,
                BackColor = Color.FromArgb(45, 45, 65),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Thêm phần chọn ảnh
            Label imageLabel = new Label
            {
                Text = "Ảnh vật phẩm:",
                ForeColor = Color.White,
                Location = new Point(20, 280),
                AutoSize = true
            };

            Button selectImageButton = new Button
            {
                Text = "Chọn ảnh",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(120, 280),
                Width = 100,
                Height = 35
            };

            // PictureBox để preview ảnh
            imagePreviewBox = new PictureBox
            {
                Location = new Point(120, 330),
                Size = new Size(200, 200),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(45, 45, 65)
            };

            Button addButton = new Button
            {
                Text = "Thêm",
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(120, 550),
                Width = 100,
                Height = 35
            };

            Button cancelButton = new Button
            {
                Text = "Hủy",
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(230, 550),
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
                imageLabel, selectImageButton,
                imagePreviewBox,
                addButton, cancelButton
            });

            // Add panel to form
            this.Controls.Add(mainPanel);

            // Event handlers
            selectImageButton.Click += (s, e) =>
            {
                try
                {
                    // Handle cross-thread operation properly
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() => OpenFileDialogThread()));
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
            };

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

                if (string.IsNullOrEmpty(selectedImagePath))
                {
                    MessageBox.Show("Vui lòng chọn ảnh vật phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tạo thư mục uploads nếu chưa tồn tại
                string uploadsDir = Path.Combine(Application.StartupPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }

                // Tạo tên file duy nhất
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(selectedImagePath)}";
                string destinationPath = Path.Combine(uploadsDir, fileName);

                // Copy file ảnh vào thư mục uploads
                File.Copy(selectedImagePath, destinationPath, true);

                // Tạo vật phẩm mới hoặc cập nhật vật phẩm hiện có
                if (newItem == null)
                {
                    newItem = new Item(
                        0, // ID sẽ được server gán
                        0, // LastestBidderId
                        nameBox.Text,
                        descBox.Text,
                        fileName, // Lưu tên file thay vì URL
                        (double)startPriceBox.Value,
                        (double)buyNowPriceBox.Value,
                        (double)startPriceBox.Value, // LastestBidPrice ban đầu bằng giá khởi điểm
                        false, // isSold
                        600000
                    );
                }
                else
                {
                    // Cập nhật thông tin vật phẩm hiện có
                    newItem.Name = nameBox.Text;
                    newItem.Description = descBox.Text;
                    newItem.StartingPrice = (double)startPriceBox.Value;
                    newItem.BuyNowPrice = (double)buyNowPriceBox.Value;
                    newItem.ImageURL = fileName;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
        }

        private void OpenFileDialogThread()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                    openFileDialog.Title = "Chọn ảnh vật phẩm";
                    openFileDialog.Multiselect = false;

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
                SafeDisposeImage(imagePreviewBox.Image);

                // Create a new image from file
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var newImage = Image.FromStream(stream);
                    imagePreviewBox.Image = new Bitmap(newImage);
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
                    if (imagePreviewBox.InvokeRequired)
                    {
                        imagePreviewBox.Invoke(new Action(() =>
                        {
                            imagePreviewBox.Image = null;
                            image.Dispose();
                        }));
                    }
                    else
                    {
                        imagePreviewBox.Image = null;
                        image.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi giải phóng ảnh: {ex.Message}");
                }
            }
        }

        public Item GetItem()
        {
            return newItem;
        }
    }
}