using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using ChatServer.Server;

namespace ChatServer
{
    public partial class ServerForm : Form
    {
        private readonly ChatServerHost _server = new ChatServerHost();

        public ServerForm()
        {
            InitializeComponent();
            _server.Log += Server_Log;
            _server.ClientsChanged += Server_ClientsChanged;
            lblAddresses.Text = "Bu bilgisayarın IP adresleri: " + GetLocalAddresses();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _server.Start((int)nudPort.Value);
                SetRunningUi(true);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(this,
                    "Sunucu başlatılamadı. Port kullanımda olabilir.\n\n" + ex.Message,
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _server.Stop();
            SetRunningUi(false);
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.Stop();
        }

        private void SetRunningUi(bool running)
        {
            btnStart.Enabled = !running;
            btnStop.Enabled = running;
            nudPort.Enabled = !running;
            lblStatus.Text = running ? "Sunucu çalışıyor, port " + _server.Port : "Sunucu kapalı";
        }

        // Sunucu olayları arka plan iş parçacığından gelir; arayüz sadece UI iş parçacığında güncellenebilir.
        private void Server_Log(object sender, string line)
        {
            RunOnUi(() => txtLog.AppendText(line + Environment.NewLine));
        }

        private void Server_ClientsChanged(object sender, EventArgs e)
        {
            RunOnUi(RefreshClientList);
        }

        private void RefreshClientList()
        {
            var clients = _server.GetClients();
            lvClients.BeginUpdate();
            lvClients.Items.Clear();
            foreach (var client in clients.OrderBy(c => c.ConnectedAt))
            {
                var item = new ListViewItem(client.UserName);
                item.SubItems.Add(client.Address);
                item.SubItems.Add(client.ConnectedAt.ToString("HH:mm:ss"));
                item.SubItems.Add(client.MessageCount.ToString());
                lvClients.Items.Add(item);
            }
            lvClients.EndUpdate();
            lblClientCount.Text = clients.Count + " istemci";
        }

        private void RunOnUi(Action action)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                try { BeginInvoke(action); }
                catch (InvalidOperationException) { } // Form kapanırken olabilir.
            }
            else
            {
                action();
            }
        }

        private static string GetLocalAddresses()
        {
            try
            {
                var addresses = Dns.GetHostAddresses(Dns.GetHostName())
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                    .Select(a => a.ToString())
                    .ToList();
                addresses.Insert(0, "127.0.0.1");
                return string.Join(", ", addresses.Distinct());
            }
            catch (SocketException)
            {
                return "127.0.0.1";
            }
        }
    }
}
