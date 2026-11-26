using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChatClient.Client;

namespace ChatClient
{
    public partial class ClientForm : Form
    {
        private const string EveryoneTarget = "Herkes";

        private readonly ChatClientService _client = new ChatClientService();
        private readonly List<ChatMessage> _history = new List<ChatMessage>();

        public ClientForm()
        {
            InitializeComponent();

            _client.MessageReceived += Client_MessageReceived;
            _client.UsersChanged += Client_UsersChanged;
            _client.ErrorReceived += Client_ErrorReceived;
            _client.Disconnected += Client_Disconnected;

            txtUserName.Text = "kullanici" + new Random().Next(10, 99);
            ResetTargets(new string[0]);
            SetConnectedUi(false);
        }

        // ---------- Bağlantı ----------

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            string userName = txtUserName.Text.Trim();

            if (host.Length == 0)
            {
                ShowWarning("Sunucu adresini girin (ör. 127.0.0.1).");
                return;
            }
            if (userName.Length == 0)
            {
                ShowWarning("Kullanıcı adı girin.");
                return;
            }

            btnConnect.Enabled = false;
            lblStatus.Text = "Bağlanılıyor: " + host + ":" + nudPort.Value + " ...";

            try
            {
                await _client.ConnectAsync(host, (int)nudPort.Value, userName);
                SetConnectedUi(true);
                AppendLocal("Sunucuya bağlanıldı. Kullanıcı adınız: " + _client.UserName);
                txtMessage.Focus();
            }
            catch (ChatClientException ex)
            {
                // Bağlantı hataları uygulamayı kapatmaz, sadece kullanıcıya bildirilir.
                SetConnectedUi(false);
                lblStatus.Text = "Bağlanılamadı";
                ShowWarning(ex.Message);
            }
        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            await _client.DisconnectAsync();
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_client.IsConnected)
            {
                _client.DisconnectAsync().Wait(500);
            }
        }

        // ---------- Mesaj gönderme ----------

        private async void btnSend_Click(object sender, EventArgs e)
        {
            await SendCurrentMessageAsync();
        }

        private async void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await SendCurrentMessageAsync();
            }
        }

        private async System.Threading.Tasks.Task SendCurrentMessageAsync()
        {
            string text = txtMessage.Text.Trim();
            if (text.Length == 0 || !_client.IsConnected)
            {
                return;
            }

            string target = cmbTarget.SelectedItem as string;
            try
            {
                if (target == null || target == EveryoneTarget)
                {
                    await _client.SendPublicAsync(text);
                }
                else
                {
                    await _client.SendPrivateAsync(target, text);
                }
                txtMessage.Clear();
            }
            catch (ChatClientException ex)
            {
                ShowWarning(ex.Message);
            }
            txtMessage.Focus();
        }

        private void lstUsers_DoubleClick(object sender, EventArgs e)
        {
            string user = lstUsers.SelectedItem as string;
            if (user == null || user == _client.UserName) return;
            cmbTarget.SelectedItem = user;
            txtMessage.Focus();
        }

        // ---------- Sunucudan gelenler (UI iş parçacığında tetiklenir) ----------

        private void Client_MessageReceived(object sender, ChatMessage message)
        {
            AppendMessage(message);
        }

        private void Client_UsersChanged(object sender, IReadOnlyList<string> users)
        {
            lstUsers.BeginUpdate();
            lstUsers.Items.Clear();
            foreach (var user in users)
            {
                lstUsers.Items.Add(user);
            }
            lstUsers.EndUpdate();
            lblUsers.Text = "Çevrimiçi kullanıcılar (" + users.Count + ")";
            ResetTargets(users.Where(u => u != _client.UserName));
        }

        private void Client_ErrorReceived(object sender, string error)
        {
            AppendLocal("Sunucu hatası: " + error);
        }

        private void Client_Disconnected(object sender, string reason)
        {
            SetConnectedUi(false);
            lstUsers.Items.Clear();
            lblUsers.Text = "Çevrimiçi kullanıcılar";
            ResetTargets(new string[0]);
            AppendLocal(reason);
        }

        // ---------- Geçmiş ----------

        private void btnSaveHistory_Click(object sender, EventArgs e)
        {
            if (_history.Count == 0)
            {
                ShowWarning("Kaydedilecek mesaj yok.");
                return;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Metin dosyası (*.txt)|*.txt";
                dialog.FileName = "sohbet-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    File.WriteAllLines(dialog.FileName, _history.Select(m => m.ToHistoryLine()), new UTF8Encoding(true));
                    lblStatus.Text = "Geçmiş kaydedildi: " + dialog.FileName;
                }
                catch (IOException ex)
                {
                    ShowWarning("Dosya yazılamadı: " + ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    ShowWarning("Bu konuma yazma izniniz yok: " + ex.Message);
                }
            }
        }

        // ---------- Yardımcılar ----------

        private void AppendLocal(string text)
        {
            AppendMessage(new ChatMessage(ChatMessageKind.Local, null, null, text, DateTime.Now));
        }

        private void AppendMessage(ChatMessage message)
        {
            _history.Add(message);

            Color headerColor;
            Color textColor = Color.Black;
            switch (message.Kind)
            {
                case ChatMessageKind.Private:
                    headerColor = Color.DarkMagenta;
                    textColor = Color.DarkMagenta;
                    break;
                case ChatMessageKind.System:
                case ChatMessageKind.Local:
                    headerColor = Color.Gray;
                    textColor = Color.Gray;
                    break;
                default:
                    headerColor = message.From == _client.UserName ? Color.SeaGreen : Color.RoyalBlue;
                    break;
            }

            AppendColored("[" + message.Time.ToString("HH:mm:ss") + "] ", Color.DarkGray, false);
            if (message.Kind == ChatMessageKind.System || message.Kind == ChatMessageKind.Local)
            {
                AppendColored(message.Text + Environment.NewLine, textColor, false);
            }
            else
            {
                AppendColored(message.Header + ": ", headerColor, true);
                AppendColored(message.Text + Environment.NewLine, textColor, false);
            }
            rtbMessages.ScrollToCaret();
        }

        private void AppendColored(string text, Color color, bool bold)
        {
            rtbMessages.SelectionStart = rtbMessages.TextLength;
            rtbMessages.SelectionLength = 0;
            rtbMessages.SelectionColor = color;
            rtbMessages.SelectionFont = new Font(rtbMessages.Font, bold ? FontStyle.Bold : FontStyle.Regular);
            rtbMessages.AppendText(text);
        }

        private void ResetTargets(IEnumerable<string> users)
        {
            string previous = cmbTarget.SelectedItem as string;
            cmbTarget.Items.Clear();
            cmbTarget.Items.Add(EveryoneTarget);
            foreach (var user in users)
            {
                cmbTarget.Items.Add(user);
            }
            cmbTarget.SelectedItem = previous != null && cmbTarget.Items.Contains(previous) ? previous : EveryoneTarget;
        }

        private void SetConnectedUi(bool connected)
        {
            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;
            txtHost.Enabled = !connected;
            nudPort.Enabled = !connected;
            txtUserName.Enabled = !connected;
            txtMessage.Enabled = connected;
            btnSend.Enabled = connected;
            cmbTarget.Enabled = connected;
            AcceptButton = connected ? null : btnConnect;

            lblConnState.Text = connected ? "● Çevrimiçi" : "● Çevrimdışı";
            lblConnState.ForeColor = connected ? Color.SeaGreen : Color.Firebrick;
            lblStatus.Text = connected
                ? "Bağlı: " + _client.Host + ":" + _client.Port + " (" + _client.UserName + ")"
                : "Bağlı değil";
            Text = connected ? "YMH219 Chat - " + _client.UserName : "YMH219 Chat - İstemci";
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(this, message, "Chat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
