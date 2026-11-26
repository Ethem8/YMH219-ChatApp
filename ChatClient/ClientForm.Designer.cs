namespace ChatClient
{
    partial class ClientForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblHost = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.rtbMessages = new System.Windows.Forms.RichTextBox();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.lstUsers = new System.Windows.Forms.ListBox();
            this.lblUsers = new System.Windows.Forms.Label();
            this.lblUsersHint = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.lblTarget = new System.Windows.Forms.Label();
            this.cmbTarget = new System.Windows.Forms.ComboBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnSaveHistory = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblConnState = new System.Windows.Forms.ToolStripStatusLabel();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.rightPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // topPanel
            //
            this.topPanel.Controls.Add(this.lblHost);
            this.topPanel.Controls.Add(this.txtHost);
            this.topPanel.Controls.Add(this.lblPort);
            this.topPanel.Controls.Add(this.nudPort);
            this.topPanel.Controls.Add(this.lblUserName);
            this.topPanel.Controls.Add(this.txtUserName);
            this.topPanel.Controls.Add(this.btnConnect);
            this.topPanel.Controls.Add(this.btnDisconnect);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Size = new System.Drawing.Size(840, 48);
            this.topPanel.Name = "topPanel";
            //
            // lblHost
            //
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(10, 16);
            this.lblHost.Name = "lblHost";
            this.lblHost.Text = "Sunucu:";
            //
            // txtHost
            //
            this.txtHost.Location = new System.Drawing.Point(64, 12);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(120, 23);
            this.txtHost.Text = "127.0.0.1";
            //
            // lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(194, 16);
            this.lblPort.Name = "lblPort";
            this.lblPort.Text = "Port:";
            //
            // nudPort
            //
            this.nudPort.Location = new System.Drawing.Point(230, 12);
            this.nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this.nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(70, 23);
            this.nudPort.Value = new decimal(new int[] { 5000, 0, 0, 0 });
            //
            // lblUserName
            //
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(312, 16);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Text = "Kullanıcı adı:";
            //
            // txtUserName
            //
            this.txtUserName.Location = new System.Drawing.Point(395, 12);
            this.txtUserName.MaxLength = 20;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(125, 23);
            //
            // btnConnect
            //
            this.btnConnect.Location = new System.Drawing.Point(530, 9);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(90, 30);
            this.btnConnect.Text = "Bağlan";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            //
            // btnDisconnect
            //
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(626, 9);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(120, 30);
            this.btnDisconnect.Text = "Bağlantıyı Kes";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            //
            // rtbMessages
            //
            this.rtbMessages.BackColor = System.Drawing.Color.White;
            this.rtbMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMessages.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.rtbMessages.Name = "rtbMessages";
            this.rtbMessages.ReadOnly = true;
            //
            // rightPanel
            //
            this.rightPanel.Controls.Add(this.lstUsers);
            this.rightPanel.Controls.Add(this.lblUsers);
            this.rightPanel.Controls.Add(this.lblUsersHint);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.rightPanel.Location = new System.Drawing.Point(650, 48);
            this.rightPanel.Size = new System.Drawing.Size(190, 424);
            //
            // lstUsers
            //
            this.lstUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstUsers.IntegralHeight = false;
            this.lstUsers.Name = "lstUsers";
            this.lstUsers.DoubleClick += new System.EventHandler(this.lstUsers_DoubleClick);
            //
            // lblUsers
            //
            this.lblUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblUsers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUsers.Height = 24;
            this.lblUsers.Name = "lblUsers";
            this.lblUsers.Text = "Çevrimiçi kullanıcılar";
            this.lblUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblUsersHint
            //
            this.lblUsersHint.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblUsersHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblUsersHint.Height = 22;
            this.lblUsersHint.Name = "lblUsersHint";
            this.lblUsersHint.Text = "Çift tık: özel mesaj gönder";
            this.lblUsersHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // bottomPanel
            //
            this.bottomPanel.Controls.Add(this.lblTarget);
            this.bottomPanel.Controls.Add(this.cmbTarget);
            this.bottomPanel.Controls.Add(this.txtMessage);
            this.bottomPanel.Controls.Add(this.btnSend);
            this.bottomPanel.Controls.Add(this.btnSaveHistory);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 472);
            this.bottomPanel.Size = new System.Drawing.Size(840, 46);
            this.bottomPanel.Name = "bottomPanel";
            //
            // lblTarget
            //
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(10, 15);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Text = "Alıcı:";
            //
            // cmbTarget
            //
            this.cmbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTarget.Location = new System.Drawing.Point(48, 11);
            this.cmbTarget.Name = "cmbTarget";
            this.cmbTarget.Size = new System.Drawing.Size(130, 23);
            //
            // txtMessage
            //
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Enabled = false;
            this.txtMessage.Location = new System.Drawing.Point(186, 11);
            this.txtMessage.MaxLength = 1000;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(432, 23);
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            //
            // btnSend
            //
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(624, 8);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(90, 30);
            this.btnSend.Text = "Gönder";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            //
            // btnSaveHistory
            //
            this.btnSaveHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveHistory.Location = new System.Drawing.Point(720, 8);
            this.btnSaveHistory.Name = "btnSaveHistory";
            this.btnSaveHistory.Size = new System.Drawing.Size(110, 30);
            this.btnSaveHistory.Text = "Geçmişi Kaydet";
            this.btnSaveHistory.UseVisualStyleBackColor = true;
            this.btnSaveHistory.Click += new System.EventHandler(this.btnSaveHistory_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatus,
                this.lblConnState});
            this.statusStrip.Name = "statusStrip";
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Bağlı değil";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblConnState
            //
            this.lblConnState.ForeColor = System.Drawing.Color.Firebrick;
            this.lblConnState.Name = "lblConnState";
            this.lblConnState.Text = "● Çevrimdışı";
            //
            // ClientForm
            //
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 540);
            this.Controls.Add(this.rtbMessages);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(780, 420);
            this.Name = "ClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "YMH219 Chat - İstemci";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.rightPanel.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.bottomPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown nudPort;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.RichTextBox rtbMessages;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.ListBox lstUsers;
        private System.Windows.Forms.Label lblUsers;
        private System.Windows.Forms.Label lblUsersHint;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.ComboBox cmbTarget;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnSaveHistory;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblConnState;
    }
}
