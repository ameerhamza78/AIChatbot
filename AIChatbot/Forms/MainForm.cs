using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using AIChatbotPro.Models;
using AIChatbotPro.Services;

namespace AIChatbotPro.Forms
{
    public class MainForm : Form
    {
        private Panel sidebar, chatPanel, inputPanel, header;
        private TextBox txtInput;
        private Button btnSend, btnNewChat;
        private ListBox historyList;

        private AIService aiService;
        private List<string> chatHistory = new List<string>();

        private int currentY = 10;

        public MainForm()
        {
            InitializeUI();
            aiService = new AIService();
        }

        private void InitializeUI()
        {
            this.Text = "🤖 AI Chatbot Pro";
            this.Size = new Size(1000, 700);
            this.BackColor = Color.FromArgb(18, 18, 18);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // 🔹 SIDEBAR
            sidebar = new Panel
            {
                Width = 230,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(28, 28, 28)
            };

            btnNewChat = new Button
            {
                Text = "＋ New Chat",
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnNewChat.FlatAppearance.BorderSize = 0;
            btnNewChat.Click += (s, e) => StartNewChat();

            historyList = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 35),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };

            sidebar.Controls.Add(historyList);
            sidebar.Controls.Add(btnNewChat);

            // 🔹 HEADER
            header = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(28, 28, 28)
            };

            var title = new Label
            {
                Text = "AI Chatbot Pro",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 15)
            };

            header.Controls.Add(title);

            // 🔹 CHAT PANEL
            chatPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(18, 18, 18)
            };

            // 🔹 INPUT PANEL
            inputPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(28, 28, 28)
            };

            txtInput = new TextBox
            {
                Width = 600,
                Location = new Point(20, 25),
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnSend = new Button
            {
                Text = "Send",
                Location = new Point(640, 23),
                Width = 120,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSend.FlatAppearance.BorderSize = 0;

            btnSend.Click += async (s, e) => await SendMessage();

            inputPanel.Controls.Add(txtInput);
            inputPanel.Controls.Add(btnSend);

            // ADD CONTROLS
            this.Controls.Add(chatPanel);
            this.Controls.Add(inputPanel);
            this.Controls.Add(header);
            this.Controls.Add(sidebar);
        }

        // 🆕 NEW CHAT
        private void StartNewChat()
        {
            chatPanel.Controls.Clear();
            currentY = 10;

            string chatName = "Chat " + (chatHistory.Count + 1);
            chatHistory.Add(chatName);
            historyList.Items.Add(chatName);
        }

        // 🚀 SEND MESSAGE
        private async Task SendMessage()
        {
            string message = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(message)) return;

            AddBubble(message, true);
            txtInput.Clear();

            var typing = AddTyping();

            AIResponse response = await aiService.GetResponse(message);

            chatPanel.Controls.Remove(typing);

            await TypeBubble(response.Content);
        }

        // 💬 USER / AI BUBBLE
        private void AddBubble(string message, bool isUser)
        {
            Label bubble = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                Padding = new Padding(12),
                Font = new Font("Segoe UI", 10),
                BackColor = isUser ? Color.FromArgb(0, 120, 215) : Color.FromArgb(45, 45, 45),
                ForeColor = Color.White
            };

            bubble.Text = message;

            bubble.Left = isUser ? chatPanel.Width - 550 : 20;
            bubble.Top = currentY;

            currentY += bubble.Height + 15;

            chatPanel.Controls.Add(bubble);
            chatPanel.ScrollControlIntoView(bubble);
        }

        // ⏳ TYPING INDICATOR
        private Label AddTyping()
        {
            Label typing = new Label
            {
                Text = "AI is typing...",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                Left = 20,
                Top = currentY
            };

            currentY += 30;

            chatPanel.Controls.Add(typing);
            return typing;
        }

        // ✨ TYPING ANIMATION
        private async Task TypeBubble(string message)
        {
            Label bubble = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                Padding = new Padding(12),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Left = 20,
                Top = currentY
            };

            chatPanel.Controls.Add(bubble);

            string text = "";

            foreach (char c in message)
            {
                text += c;
                bubble.Text = text;
                await Task.Delay(10);
            }

            currentY += bubble.Height + 15;
            chatPanel.ScrollControlIntoView(bubble);
        }
    }
}