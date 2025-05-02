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
            this.listView1 = new System.Windows.Forms.ListView();
            this.featuredAuctionPanel = new System.Windows.Forms.Panel();
            this.roomsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.sidePanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userPictureBox)).BeginInit();
            this.featuredAuctionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sidePanel
            // 
            this.sidePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(55)))));
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
            this.settingsButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            // 
            // auctionsButton
            // 
            this.auctionsButton.BackColor = System.Drawing.Color.Transparent;
            this.auctionsButton.FlatAppearance.BorderSize = 0;
            this.auctionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.auctionsButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.dashboardButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.homeButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.subLogoLabel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.logoLabel.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold);
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
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(55)))));
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
            this.dateLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.userNameLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.timeLabel.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(121, 97);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // featuredAuctionPanel
            // 
            this.featuredAuctionPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(70)))));
            this.featuredAuctionPanel.Controls.Add(this.roomsPanel);
            this.featuredAuctionPanel.Location = new System.Drawing.Point(267, 57);
            this.featuredAuctionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.featuredAuctionPanel.Name = "featuredAuctionPanel";
            this.featuredAuctionPanel.Size = new System.Drawing.Size(1045, 633);
            this.featuredAuctionPanel.TabIndex = 5;
            // 
            // roomsPanel
            // 
            this.roomsPanel.AutoScroll = true;
            this.roomsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(50)))));
            this.roomsPanel.Location = new System.Drawing.Point(0, 0);
            this.roomsPanel.Name = "roomsPanel";
            this.roomsPanel.Padding = new System.Windows.Forms.Padding(10);
            this.roomsPanel.Size = new System.Drawing.Size(1045, 630);
            this.roomsPanel.TabIndex = 6;
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
        public string userPictureUrl = "https://www.w3schools.com/howto/img_avatar.png";
    }
}