namespace Client.Forms
{
    partial class FormLobby
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sidePanel = new System.Windows.Forms.Panel();
            this.settingsButton = new System.Windows.Forms.Button();
            this.auctionsButton = new System.Windows.Forms.Button();
            this.dashboardButton = new System.Windows.Forms.Button();
            this.homeButton = new System.Windows.Forms.Button();
            this.subLogoLabel = new System.Windows.Forms.Label();
            this.logoLabel = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.userPictureBox = new System.Windows.Forms.PictureBox();
            this.dateLabel = new System.Windows.Forms.Label();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.clockTimer = new System.Windows.Forms.Timer(this.components);
            this.auctionTimer = new System.Windows.Forms.Timer(this.components);
            this.featuredAuctionPanel = new System.Windows.Forms.Panel();
            this.roomsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.auctionMainPanel = new System.Windows.Forms.Panel();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.itemPictureBox = new System.Windows.Forms.PictureBox();
            this.itemNameLabel = new System.Windows.Forms.Label();
            this.itemDescLabel = new System.Windows.Forms.Label();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.auctionInfoPanel = new System.Windows.Forms.Panel();
            this.currentPriceLabel = new System.Windows.Forms.Label();
            this.lastBidderLabel = new System.Windows.Forms.Label();
            this.timeRemainingLabel = new System.Windows.Forms.Label();
            this.bidInput = new System.Windows.Forms.NumericUpDown();
            this.placeBidButton = new System.Windows.Forms.Button();
            this.soldItemsPanel = new System.Windows.Forms.Panel();
            this.soldItemsTitleLabel = new System.Windows.Forms.Label();
            this.roomInfoPanel = new System.Windows.Forms.Panel();
            this.roomNameLabel = new System.Windows.Forms.Label();
            this.leaveRoomButton = new System.Windows.Forms.Button();
            this.sidePanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userPictureBox)).BeginInit();
            this.featuredAuctionPanel.SuspendLayout();
            this.auctionMainPanel.SuspendLayout();
            this.leftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemPictureBox)).BeginInit();
            this.rightPanel.SuspendLayout();
            this.auctionInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bidInput)).BeginInit();
            this.soldItemsPanel.SuspendLayout();
            this.roomInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidePanel
            // 
            this.sidePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(40)))));
            this.sidePanel.Controls.Add(this.settingsButton);
            this.sidePanel.Controls.Add(this.auctionsButton);
            this.sidePanel.Controls.Add(this.dashboardButton);
            this.sidePanel.Controls.Add(this.homeButton);
            this.sidePanel.Controls.Add(this.subLogoLabel);
            this.sidePanel.Controls.Add(this.logoLabel);
            this.sidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidePanel.Location = new System.Drawing.Point(0, 0);
            this.sidePanel.Margin = new System.Windows.Forms.Padding(4);
            this.sidePanel.Name = "sidePanel";
            this.sidePanel.Size = new System.Drawing.Size(267, 690);
            this.sidePanel.TabIndex = 0;
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsButton.FlatAppearance.BorderSize = 0;
            this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsButton.ForeColor = System.Drawing.Color.White;
            this.settingsButton.Location = new System.Drawing.Point(0, 254);
            this.settingsButton.Margin = new System.Windows.Forms.Padding(4);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.settingsButton.Size = new System.Drawing.Size(267, 49);
            this.settingsButton.TabIndex = 8;
            this.settingsButton.Text = "⚙️ Profile Settings";
            this.settingsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsButton.UseVisualStyleBackColor = false;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // auctionsButton
            // 
            this.auctionsButton.BackColor = System.Drawing.Color.Transparent;
            this.auctionsButton.FlatAppearance.BorderSize = 0;
            this.auctionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.auctionsButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.auctionsButton.ForeColor = System.Drawing.Color.White;
            this.auctionsButton.Location = new System.Drawing.Point(0, 197);
            this.auctionsButton.Margin = new System.Windows.Forms.Padding(4);
            this.auctionsButton.Name = "auctionsButton";
            this.auctionsButton.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.auctionsButton.Size = new System.Drawing.Size(267, 49);
            this.auctionsButton.TabIndex = 4;
            this.auctionsButton.Text = "📥 Join Room";
            this.auctionsButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.auctionsButton.UseVisualStyleBackColor = false;
            // 
            // dashboardButton
            // 
            this.dashboardButton.BackColor = System.Drawing.Color.Transparent;
            this.dashboardButton.FlatAppearance.BorderSize = 0;
            this.dashboardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dashboardButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dashboardButton.ForeColor = System.Drawing.Color.White;
            this.dashboardButton.Location = new System.Drawing.Point(0, 135);
            this.dashboardButton.Margin = new System.Windows.Forms.Padding(4);
            this.dashboardButton.Name = "dashboardButton";
            this.dashboardButton.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.dashboardButton.Size = new System.Drawing.Size(267, 49);
            this.dashboardButton.TabIndex = 3;
            this.dashboardButton.Text = "➕ Create Room";
            this.dashboardButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.dashboardButton.UseVisualStyleBackColor = false;
            this.dashboardButton.Click += new System.EventHandler(this.dashboardButton_Click);
            // 
            // homeButton
            // 
            this.homeButton.BackColor = System.Drawing.Color.Transparent;
            this.homeButton.FlatAppearance.BorderSize = 0;
            this.homeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.homeButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homeButton.ForeColor = System.Drawing.Color.White;
            this.homeButton.Location = new System.Drawing.Point(0, 74);
            this.homeButton.Margin = new System.Windows.Forms.Padding(4);
            this.homeButton.Name = "homeButton";
            this.homeButton.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.homeButton.Size = new System.Drawing.Size(267, 49);
            this.homeButton.TabIndex = 2;
            this.homeButton.Text = "🏠 Home";
            this.homeButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.homeButton.UseVisualStyleBackColor = false;
            // 
            // subLogoLabel
            // 
            this.subLogoLabel.AutoSize = true;
            this.subLogoLabel.BackColor = System.Drawing.Color.Transparent;
            this.subLogoLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subLogoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(171)))));
            this.subLogoLabel.Location = new System.Drawing.Point(175, 21);
            this.subLogoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.subLogoLabel.Name = "subLogoLabel";
            this.subLogoLabel.Size = new System.Drawing.Size(84, 23);
            this.subLogoLabel.TabIndex = 1;
            this.subLogoLabel.Text = "Auctions";
            // 
            // logoLabel
            // 
            this.logoLabel.AutoSize = true;
            this.logoLabel.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logoLabel.ForeColor = System.Drawing.Color.White;
            this.logoLabel.Location = new System.Drawing.Point(13, 12);
            this.logoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.logoLabel.Name = "logoLabel";
            this.logoLabel.Size = new System.Drawing.Size(157, 40);
            this.logoLabel.TabIndex = 0;
            this.logoLabel.Text = "TEMPLE";
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(40)))));
            this.headerPanel.Controls.Add(this.userPictureBox);
            this.headerPanel.Controls.Add(this.dateLabel);
            this.headerPanel.Controls.Add(this.userNameLabel);
            this.headerPanel.Controls.Add(this.timeLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(267, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(4);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1045, 62);
            this.headerPanel.TabIndex = 1;
            // 
            // userPictureBox
            // 
            this.userPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(171)))));
            this.userPictureBox.Location = new System.Drawing.Point(822, 12);
            this.userPictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.userPictureBox.Name = "userPictureBox";
            this.userPictureBox.Size = new System.Drawing.Size(40, 37);
            this.userPictureBox.TabIndex = 1;
            this.userPictureBox.TabStop = false;
            this.userPictureBox.Click += new System.EventHandler(this.userPictureBox_Click);
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.BackColor = System.Drawing.Color.Transparent;
            this.dateLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLabel.ForeColor = System.Drawing.Color.White;
            this.dateLabel.Location = new System.Drawing.Point(315, 33);
            this.dateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(231, 19);
            this.dateLabel.TabIndex = 4;
            this.dateLabel.Text = "Thursday, December 19, 2019";
            // 
            // userNameLabel
            // 
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userNameLabel.ForeColor = System.Drawing.Color.NavajoWhite;
            this.userNameLabel.Location = new System.Drawing.Point(870, 21);
            this.userNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.userNameLabel.MaximumSize = new System.Drawing.Size(300, 0);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(83, 19);
            this.userNameLabel.TabIndex = 0;
            this.userNameLabel.Text = "Username";
            this.userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.userNameLabel.Click += new System.EventHandler(this.userNameLabel_Click);
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.BackColor = System.Drawing.Color.Transparent;
            this.timeLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLabel.ForeColor = System.Drawing.Color.White;
            this.timeLabel.Location = new System.Drawing.Point(386, 9);
            this.timeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(105, 29);
            this.timeLabel.TabIndex = 3;
            this.timeLabel.Text = "19:12:06";
            // 
            // clockTimer
            // 
            this.clockTimer.Interval = 1000;
            this.clockTimer.Tick += new System.EventHandler(this.clockTimer_Tick);
            // 
            // auctionTimer
            // 
            this.auctionTimer.Interval = 1000;
            this.auctionTimer.Tick += new System.EventHandler(this.auctionTimer_Tick);
            // 
            // featuredAuctionPanel
            // 
            this.featuredAuctionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.featuredAuctionPanel.Controls.Add(this.roomsPanel);
            this.featuredAuctionPanel.Controls.Add(this.auctionMainPanel);
            this.featuredAuctionPanel.Location = new System.Drawing.Point(267, 62);
            this.featuredAuctionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.featuredAuctionPanel.Name = "featuredAuctionPanel";
            this.featuredAuctionPanel.Size = new System.Drawing.Size(1045, 628);
            this.featuredAuctionPanel.TabIndex = 5;
            // 
            // roomsPanel
            // 
            this.roomsPanel.AutoScroll = true;
            this.roomsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.roomsPanel.Location = new System.Drawing.Point(0, 0);
            this.roomsPanel.Name = "roomsPanel";
            this.roomsPanel.Padding = new System.Windows.Forms.Padding(10);
            this.roomsPanel.Size = new System.Drawing.Size(1045, 628);
            this.roomsPanel.TabIndex = 6;
            // 
            // auctionMainPanel
            // 
            this.auctionMainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.auctionMainPanel.Controls.Add(this.leftPanel);
            this.auctionMainPanel.Controls.Add(this.rightPanel);
            this.auctionMainPanel.Controls.Add(this.roomInfoPanel);
            this.auctionMainPanel.Location = new System.Drawing.Point(0, 0);
            this.auctionMainPanel.Name = "auctionMainPanel";
            this.auctionMainPanel.Size = new System.Drawing.Size(1045, 628);
            this.auctionMainPanel.TabIndex = 7;
            this.auctionMainPanel.Visible = false;
            // 
            // leftPanel
            // 
            this.leftPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.leftPanel.Controls.Add(this.itemPictureBox);
            this.leftPanel.Controls.Add(this.itemNameLabel);
            this.leftPanel.Controls.Add(this.itemDescLabel);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 60);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Padding = new System.Windows.Forms.Padding(20);
            this.leftPanel.Size = new System.Drawing.Size(400, 568);
            this.leftPanel.TabIndex = 1;
            // 
            // itemPictureBox
            // 
            this.itemPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(70)))));
            this.itemPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.itemPictureBox.Location = new System.Drawing.Point(20, 20);
            this.itemPictureBox.Name = "itemPictureBox";
            this.itemPictureBox.Size = new System.Drawing.Size(360, 260);
            this.itemPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.itemPictureBox.TabIndex = 0;
            this.itemPictureBox.TabStop = false;
            // 
            // itemNameLabel
            // 
            this.itemNameLabel.AutoSize = true;
            this.itemNameLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemNameLabel.ForeColor = System.Drawing.Color.White;
            this.itemNameLabel.Location = new System.Drawing.Point(20, 300);
            this.itemNameLabel.Name = "itemNameLabel";
            this.itemNameLabel.Size = new System.Drawing.Size(120, 30);
            this.itemNameLabel.TabIndex = 1;
            this.itemNameLabel.Text = "Item Name";
            // 
            // itemDescLabel
            // 
            this.itemDescLabel.AutoSize = false;
            this.itemDescLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemDescLabel.ForeColor = System.Drawing.Color.LightGray;
            this.itemDescLabel.Location = new System.Drawing.Point(20, 340);
            this.itemDescLabel.Name = "itemDescLabel";
            this.itemDescLabel.Size = new System.Drawing.Size(360, 200);
            this.itemDescLabel.TabIndex = 2;
            this.itemDescLabel.Text = "Item Description";
            // 
            // rightPanel
            // 
            this.rightPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.rightPanel.Controls.Add(this.auctionInfoPanel);
            this.rightPanel.Controls.Add(this.soldItemsPanel);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(400, 60);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(645, 568);
            this.rightPanel.TabIndex = 2;
            // 
            // auctionInfoPanel
            // 
            this.auctionInfoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(60)))));
            this.auctionInfoPanel.Controls.Add(this.currentPriceLabel);
            this.auctionInfoPanel.Controls.Add(this.lastBidderLabel);
            this.auctionInfoPanel.Controls.Add(this.timeRemainingLabel);
            this.auctionInfoPanel.Controls.Add(this.bidInput);
            this.auctionInfoPanel.Controls.Add(this.placeBidButton);
            this.auctionInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.auctionInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.auctionInfoPanel.Name = "auctionInfoPanel";
            this.auctionInfoPanel.Padding = new System.Windows.Forms.Padding(20);
            this.auctionInfoPanel.Size = new System.Drawing.Size(645, 220);
            this.auctionInfoPanel.TabIndex = 0;
            // 
            // currentPriceLabel
            // 
            this.currentPriceLabel.AutoSize = true;
            this.currentPriceLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentPriceLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(200)))), ((int)(((byte)(100)))));
            this.currentPriceLabel.Location = new System.Drawing.Point(20, 20);
            this.currentPriceLabel.Name = "currentPriceLabel";
            this.currentPriceLabel.Size = new System.Drawing.Size(180, 25);
            this.currentPriceLabel.TabIndex = 0;
            this.currentPriceLabel.Text = "Giá hiện tại: 0 VND";
            // 
            // lastBidderLabel
            // 
            this.lastBidderLabel.AutoSize = true;
            this.lastBidderLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastBidderLabel.ForeColor = System.Drawing.Color.White;
            this.lastBidderLabel.Location = new System.Drawing.Point(20, 55);
            this.lastBidderLabel.Name = "lastBidderLabel";
            this.lastBidderLabel.Size = new System.Drawing.Size(150, 20);
            this.lastBidderLabel.TabIndex = 1;
            this.lastBidderLabel.Text = "Người đấu giá: None";
            // 
            // timeRemainingLabel
            // 
            this.timeRemainingLabel.AutoSize = true;
            this.timeRemainingLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeRemainingLabel.ForeColor = System.Drawing.Color.White;
            this.timeRemainingLabel.Location = new System.Drawing.Point(20, 85);
            this.timeRemainingLabel.Name = "timeRemainingLabel";
            this.timeRemainingLabel.Size = new System.Drawing.Size(150, 20);
            this.timeRemainingLabel.TabIndex = 2;
            this.timeRemainingLabel.Text = "Thời gian còn lại: 00:00:00";
            // 
            // bidInput
            // 
            this.bidInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(70)))));
            this.bidInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bidInput.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bidInput.ForeColor = System.Drawing.Color.White;
            this.bidInput.Location = new System.Drawing.Point(20, 120);
            this.bidInput.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.bidInput.Name = "bidInput";
            this.bidInput.Size = new System.Drawing.Size(200, 27);
            this.bidInput.TabIndex = 3;
            // 
            // placeBidButton
            // 
            this.placeBidButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
            this.placeBidButton.FlatAppearance.BorderSize = 0;
            this.placeBidButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(160)))), ((int)(((byte)(210)))));
            this.placeBidButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.placeBidButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.placeBidButton.ForeColor = System.Drawing.Color.White;
            this.placeBidButton.Location = new System.Drawing.Point(230, 120);
            this.placeBidButton.Name = "placeBidButton";
            this.placeBidButton.Size = new System.Drawing.Size(120, 35);
            this.placeBidButton.TabIndex = 4;
            this.placeBidButton.Text = "Đặt giá";
            this.placeBidButton.UseVisualStyleBackColor = false;
            // 
            // roomInfoPanel
            // 
            this.roomInfoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(40)))));
            this.roomInfoPanel.Controls.Add(this.roomNameLabel);
            this.roomInfoPanel.Controls.Add(this.leaveRoomButton);
            this.roomInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.roomInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.roomInfoPanel.Name = "roomInfoPanel";
            this.roomInfoPanel.Size = new System.Drawing.Size(1045, 60);
            this.roomInfoPanel.TabIndex = 0;
            // 
            // roomNameLabel
            // 
            this.roomNameLabel.AutoSize = true;
            this.roomNameLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roomNameLabel.ForeColor = System.Drawing.Color.White;
            this.roomNameLabel.Location = new System.Drawing.Point(15, 15);
            this.roomNameLabel.Name = "roomNameLabel";
            this.roomNameLabel.Size = new System.Drawing.Size(150, 30);
            this.roomNameLabel.TabIndex = 0;
            this.roomNameLabel.Text = "Phòng đấu giá";
            // 
            // leaveRoomButton
            // 
            this.leaveRoomButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.leaveRoomButton.FlatAppearance.BorderSize = 0;
            this.leaveRoomButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.leaveRoomButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.leaveRoomButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.leaveRoomButton.ForeColor = System.Drawing.Color.White;
            this.leaveRoomButton.Location = new System.Drawing.Point(905, 15);
            this.leaveRoomButton.Name = "leaveRoomButton";
            this.leaveRoomButton.Size = new System.Drawing.Size(120, 35);
            this.leaveRoomButton.TabIndex = 1;
            this.leaveRoomButton.Text = "Thoát phòng";
            this.leaveRoomButton.UseVisualStyleBackColor = false;
            // 
            // FormLobby
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(65)))));
            this.ClientSize = new System.Drawing.Size(1312, 690);
            this.Controls.Add(this.featuredAuctionPanel);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.sidePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "FormLobby";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Temple Auctions";
            this.sidePanel.ResumeLayout(false);
            this.sidePanel.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userPictureBox)).EndInit();
            this.featuredAuctionPanel.ResumeLayout(false);
            this.auctionMainPanel.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.leftPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemPictureBox)).EndInit();
            this.rightPanel.ResumeLayout(false);
            this.auctionInfoPanel.ResumeLayout(false);
            this.auctionInfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bidInput)).EndInit();
            this.soldItemsPanel.ResumeLayout(false);
            this.soldItemsPanel.PerformLayout();
            this.roomInfoPanel.ResumeLayout(false);
            this.roomInfoPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel sidePanel;
        private System.Windows.Forms.Label logoLabel;
        private System.Windows.Forms.Label subLogoLabel;
        private System.Windows.Forms.Button homeButton;
        private System.Windows.Forms.Button dashboardButton;
        private System.Windows.Forms.Button auctionsButton;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.PictureBox userPictureBox;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.Timer clockTimer;
        private System.Windows.Forms.Timer auctionTimer;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Panel featuredAuctionPanel;
        private System.Windows.Forms.FlowLayoutPanel roomsPanel;
        private System.Windows.Forms.Panel auctionMainPanel;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.PictureBox itemPictureBox;
        private System.Windows.Forms.Label itemNameLabel;
        private System.Windows.Forms.Label itemDescLabel;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Panel auctionInfoPanel;
        private System.Windows.Forms.Label currentPriceLabel;
        private System.Windows.Forms.Label lastBidderLabel;
        private System.Windows.Forms.Label timeRemainingLabel;
        private System.Windows.Forms.NumericUpDown bidInput;
        private System.Windows.Forms.Button placeBidButton;
        private System.Windows.Forms.Panel soldItemsPanel;
        private System.Windows.Forms.Label soldItemsTitleLabel;
        private System.Windows.Forms.Panel roomInfoPanel;
        private System.Windows.Forms.Label roomNameLabel;
        private System.Windows.Forms.Button leaveRoomButton;
        public string userPictureUrl = "https://www.w3schools.com/howto/img_avatar.png";
    }
}