using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Tank_Practice
{
    public partial class ConnectorForm : Form
    {
        public Form1 parent;
        bool serverOpen;
        public bool connectReq;
        public ConnectorForm()
        {
            InitializeComponent();
            textBox1.Text = "21212";
            textBox2.Text = "127.0.0.1";
            serverOpen = connectReq = false;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            ConnectLoglistBox.Items.Add("Server Open Started.");
            try
            {
                btnConnect.Enabled = false;
                btnOpen.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;

                parent.openServer();
                ConnectLoglistBox.Items.Add("Waiting for other player...");
                serverOpen = true;
            }
            catch(Exception ex)
            {
                ConnectLoglistBox.Items.Add("Error! check port number");
            }

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectLoglistBox.Items.Add("Connect Request Started...");
            try
            {
                btnConnect.Enabled = false;
                btnOpen.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                parent.connectServer();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Connection Failed! check your Address.");
            }
        }

        public void setserverconnFlag()
        {
            parent.server_connected = true;
        }

        public void setclientconnFlag()
        {
            parent.client_connected = true;
        }

        private void ConnectorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serverOpen && !connectReq)
            {
                parent.server.StopServer();
            }
        }
    }
}
