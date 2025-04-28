using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineAuctionSystem
{
    public partial class FormLobby : Form
    {
        private Panel sidePanel;
        private Panel headerPanel;
        private Button homeButton;
        private Button dashboardButton;
        private Button auctionsButton;
        private Button itemsButton;
        private Button biddersButton;
        private Button paymentsButton;
        private Button settingsButton;
        private Label logoLabel;
        private Label subLogoLabel;
        private Label timeLabel;
        private Label dateLabel;
        private PictureBox logoBox;
        private PictureBox userPictureBox;
        private Label userNameLabel;
        private Button menuButton;
        private Button notificationButton;
        private Timer timer;

        // New auction-specific components
        private Panel featuredAuctionPanel;
        private Label featuredLabel;
        private Label currentBidLabel;
        private Label timeLeftLabel;
        private Button bidNowButton;
        private PictureBox itemImageBox;
        private Timer auctionTimer;
        private int secondsLeft = 3600; // 1 hour auction countdown

        public FormLobby()
        {
            InitializeComponent();
            InitializeCustomComponents();
            StartTimer();
            StartAuctionTimer();
        }

        private void InitializeComponent()
        {
            this.Text = "Temple Auctions";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(45, 45, 65);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Icon = SystemIcons.Application;
        }

        private void InitializeCustomComponents()
        {
            // Side Panel
            sidePanel = new Panel
            {
                BackColor = Color.FromArgb(30, 30, 55),
                Dock = DockStyle.Left,
                Width = 200
            };
            this.Controls.Add(sidePanel);

            // Header Panel
            headerPanel = new Panel
            {
                BackColor = Color.FromArgb(30, 30, 55),
                Dock = DockStyle.Top,
                Height = 50
            };
            this.Controls.Add(headerPanel);

            // Logo
            logoLabel = new Label
            {
                Text = "TEMPLE",
                ForeColor = Color.White,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            sidePanel.Controls.Add(logoLabel);

            // Sub Logo
            subLogoLabel = new Label
            {
                Text = "Auctions",
                ForeColor = Color.FromArgb(255, 128, 171),
                Font = new Font("Arial", 12, FontStyle.Regular),
                Location = new Point(120, 17),
                AutoSize = true
            };
            sidePanel.Controls.Add(subLogoLabel);

            // Menu Button
            menuButton = new Button
            {
                Text = "≡",
                ForeColor = Color.White,
                Font = new Font("Arial", 20, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(920, 10),
                Size = new Size(40, 30),
                BackColor = Color.Transparent
            };
            menuButton.FlatAppearance.BorderSize = 0;
            headerPanel.Controls.Add(menuButton);

            // Notification Button
            notificationButton = new Button
            {
                Text = "🔔",
                ForeColor = Color.White,
                Font = new Font("Arial", 15, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(880, 10),
                Size = new Size(40, 30),
                BackColor = Color.Transparent
            };
            notificationButton.FlatAppearance.BorderSize = 0;
            headerPanel.Controls.Add(notificationButton);

            // User Picture
            userPictureBox = new PictureBox
            {
                Size = new Size(30, 30),
                Location = new Point(840, 10),
                BackColor = Color.FromArgb(255, 128, 171),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            userPictureBox.Paint += (sender, e) =>
            {
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(0, 0, userPictureBox.Width - 1, userPictureBox.Height - 1);
                userPictureBox.Region = new Region(gp);
            };
            headerPanel.Controls.Add(userPictureBox);

            // User Name
            userNameLabel = new Label
            {
                Text ="Temple peter",
                ForeColor = Color.NavajoWhite,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Location = new Point(750, 17),
                AutoSize = true
            };
            headerPanel.Controls.Add(userNameLabel);

            // Navigation Buttons - Updated for auction system
            homeButton = CreateNavigationButton("Home", "🏠", 60);
            dashboardButton = CreateNavigationButton("Dashboard", "📊", 110);
            auctionsButton = CreateNavigationButton("Auctions", "🔨", 160);
            itemsButton = CreateNavigationButton("Items", "📦", 210);
            biddersButton = CreateNavigationButton("Bidders", "👥", 260);
            paymentsButton = CreateNavigationButton("Payments", "💰", 310);
            settingsButton = CreateNavigationButton("Settings", "⚙️", 360);

            // Content Area - Logo and Time
            logoBox = new PictureBox
            {
                Size = new Size(300, 100),
                Location = new Point(350, 60),
                BackColor = Color.Transparent
            };
            logoBox.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    // Draw diamond shape
                    Point[] points = new Point[] {
                        new Point(50, 0),
                        new Point(100, 50),
                        new Point(50, 100),
                        new Point(0, 50)
                    };
                    path.AddPolygon(points);

                    // Fill with gradient
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        new Point(0, 0), new Point(100, 100),
                        Color.FromArgb(180, 100, 255), Color.FromArgb(255, 128, 171)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    // Draw "T" in the center
                    using (Font font = new Font("Arial", 30, FontStyle.Bold))
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        e.Graphics.DrawString("T", font, textBrush, new PointF(35, 25));
                    }
                }

                // Draw the text
                using (Font font = new Font("Arial", 30, FontStyle.Bold))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                {
                    e.Graphics.DrawString("TEMPLE", font, textBrush, new PointF(110, 20));
                }

                using (Font font = new Font("Arial", 18, FontStyle.Regular))
                using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(255, 128, 171)))
                {
                    e.Graphics.DrawString("Auctions", font, textBrush, new PointF(110, 60));
                }
            };
            this.Controls.Add(logoBox);

            // Time Label
            timeLabel = new Label
            {
                Text = "19:12:06",
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(815, 60),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(timeLabel);

            // Date Label
            dateLabel = new Label
            {
                Text = "Thursday, December 19, 2019",
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Location = new Point(760, 80),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(dateLabel);

            // Featured Auction Panel - New component
            featuredAuctionPanel = new Panel
            {
                BackColor = Color.FromArgb(40, 40, 70),
                Location = new Point(220, 170),
                Size = new Size(760, 380),
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(featuredAuctionPanel);

            // Featured Label
            featuredLabel = new Label
            {
                Text = "Featured Auction: Vintage Gold Watch",
                ForeColor = Color.White,
                Font = new Font("Arial", 18, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            featuredAuctionPanel.Controls.Add(featuredLabel);

            // Current Bid Label
            currentBidLabel = new Label
            {
                Text = "Current Bid: $1,250.00",
                ForeColor = Color.FromArgb(255, 215, 0), // Gold color
                Font = new Font("Arial", 24, FontStyle.Bold),
                Location = new Point(20, 70),
                AutoSize = true
            };
            featuredAuctionPanel.Controls.Add(currentBidLabel);

            // Time Left Label
            timeLeftLabel = new Label
            {
                Text = "Time Left: 01:00:00",
                ForeColor = Color.White,
                Font = new Font("Arial", 16, FontStyle.Regular),
                Location = new Point(20, 120),
                AutoSize = true
            };
            featuredAuctionPanel.Controls.Add(timeLeftLabel);

            // Bid Now Button
            bidNowButton = new Button
            {
                Text = "Place Bid",
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 170),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(255, 128, 171)
            };
            bidNowButton.FlatAppearance.BorderSize = 0;
            bidNowButton.Click += (sender, e) =>
            {
                // Simulate bid increment
                string currentBid = currentBidLabel.Text.Replace("Current Bid: $", "").Replace(",", "");
                if (double.TryParse(currentBid, out double bidAmount))
                {
                    bidAmount += 50;
                    currentBidLabel.Text = $"Current Bid: ${bidAmount:N2}";
                }

                // Flash the bid amount to indicate change
                FlashBidAmount();
            };
            featuredAuctionPanel.Controls.Add(bidNowButton);

            // Item Image Box
            itemImageBox = new PictureBox
            {
                Size = new Size(300, 300),
                Location = new Point(430, 40),
                BackColor = Color.FromArgb(50, 50, 80),
                BorderStyle = BorderStyle.FixedSingle
            };
            itemImageBox.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Draw a placeholder watch image
                using (Pen pen = new Pen(Color.Gold, 3))
                {
                    // Watch case
                    e.Graphics.DrawEllipse(pen, 100, 100, 100, 100);

                    // Watch hands
                    e.Graphics.DrawLine(pen, 150, 150, 150, 110);
                    e.Graphics.DrawLine(pen, 150, 150, 180, 150);

                    // Watch band
                    e.Graphics.DrawLine(pen, 150, 100, 150, 70);
                    e.Graphics.DrawLine(pen, 150, 200, 150, 230);
                }

                // Draw "Vintage Gold Watch" text
                using (Font font = new Font("Arial", 14, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    e.Graphics.DrawString("Vintage Gold Watch", font, brush, new PointF(80, 250));
                }
            };
            featuredAuctionPanel.Controls.Add(itemImageBox);

            // Add bid history
            Label bidHistoryLabel = new Label
            {
                Text = "Recent Bids:",
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, 230),
                AutoSize = true
            };
            featuredAuctionPanel.Controls.Add(bidHistoryLabel);

            // Sample bid history entries
            string[] bidders = { "User_4521", "Premium_Bidder", "Collector99", "AuctionKing" };
            double[] amounts = { 1200, 1150, 1100, 1050 };

            for (int i = 0; i < bidders.Length; i++)
            {
                Label bidEntry = new Label
                {
                    Text = $"{bidders[i]}: ${amounts[i]:N2}",
                    ForeColor = Color.LightGray,
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    Location = new Point(20, 260 + (i * 25)),
                    AutoSize = true
                };
                featuredAuctionPanel.Controls.Add(bidEntry);
            }
        }

        private void FlashBidAmount()
        {
            // Create a simple animation to flash the bid amount when changed
            Color originalColor = currentBidLabel.ForeColor;
            Timer flashTimer = new Timer { Interval = 100 };
            int flashCount = 0;

            flashTimer.Tick += (s, e) =>
            {
                if (flashCount % 2 == 0)
                {
                    currentBidLabel.ForeColor = Color.White;
                }
                else
                {
                    currentBidLabel.ForeColor = originalColor;
                }

                flashCount++;
                if (flashCount >= 6)
                {
                    currentBidLabel.ForeColor = originalColor;
                    flashTimer.Stop();
                }
            };

            flashTimer.Start();
        }

        private Button CreateNavigationButton(string text, string icon, int topPosition)
        {
            Button button = new Button
            {
                Text = $" {icon} {text}",
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, topPosition),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 45, 65);
            button.Padding = new Padding(10, 0, 0, 0);
            sidePanel.Controls.Add(button);
            return button;
        }

        private void StartTimer()
        {
            timer = new Timer
            {
                Interval = 1000
            };
            timer.Tick += (sender, e) =>
            {
                DateTime now = DateTime.Now;
                timeLabel.Text = now.ToString("HH:mm:ss");
                dateLabel.Text = now.ToString("dddd, MMMM d, yyyy");
            };
            timer.Start();
        }

        private void StartAuctionTimer()
        {
            auctionTimer = new Timer
            {
                Interval = 1000
            };
            auctionTimer.Tick += (sender, e) =>
            {
                secondsLeft--;
                if (secondsLeft <= 0)
                {
                    timeLeftLabel.Text = "Auction Ended";
                    bidNowButton.Enabled = false;
                    bidNowButton.BackColor = Color.Gray;
                    auctionTimer.Stop();
                }
                else
                {
                    int hours = secondsLeft / 3600;
                    int minutes = (secondsLeft % 3600) / 60;
                    int seconds = secondsLeft % 60;
                    timeLeftLabel.Text = $"Time Left: {hours:D2}:{minutes:D2}:{seconds:D2}";

                    // Make the time flash red when less than 5 minutes
                    if (secondsLeft < 300)
                    {
                        timeLeftLabel.ForeColor = secondsLeft % 2 == 0 ? Color.Red : Color.White;
                    }
                }
            };
            auctionTimer.Start();
        }

    }
}