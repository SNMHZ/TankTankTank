using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Tank_Practice
{
    public class m_Server
    {
        public class AsyncObject
        {
            public byte[] Buffer;
            public Socket WorkingSocket;
            public AsyncObject(int bufferSize)
            {
                this.Buffer = new byte[bufferSize];
            }
        }
        private Socket m_ConnectedClient = null;
        private Socket m_ServerSocket = null;
        private AsyncCallback m_ReceiveHandler;
        private AsyncCallback m_SendHandler;
        private AsyncCallback m_AcceptHandler;
        private int port;
        public ConnectorForm connFrm;
        public m_Server(ConnectorForm _connFrm)
        {
            m_ReceiveHandler = new AsyncCallback(handleReceivedData);
            m_SendHandler = new AsyncCallback(handleSentData);
            m_AcceptHandler = new AsyncCallback(handleClientConnectionRequest);
            connFrm = _connFrm;
            port = Int32.Parse(connFrm.textBox1.Text);
            connFrm.ConnectLoglistBox.Items.Add("수신 대기중인 포트 번호 : " + port);
        }

        public void StartServer()
        {
            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            m_ServerSocket.Listen(5);
            m_ServerSocket.BeginAccept(m_AcceptHandler, null);
            connFrm.ConnectLoglistBox.Items.Add("Server Opened!!");
        }

        public void StopServer()
        {
            m_ServerSocket.Close();
        }

        public void SendMessage(string message)
        {
            AsyncObject ao = new AsyncObject(1);
            ao.Buffer = Encoding.Unicode.GetBytes(message);
            ao.WorkingSocket = m_ConnectedClient;
            try
            {
                m_ConnectedClient.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_SendHandler, ao);
            }
            catch (Exception ex)
            {
                //ChatListBox.Items.Add("전송 도중 오류 발생 : " + ex.Message);
            }
        }

        private void handleClientConnectionRequest(IAsyncResult ar)
        {
            Socket sockClient;
            try
            {
                sockClient = m_ServerSocket.EndAccept(ar);
                connFrm.ConnectLoglistBox.Items.Add("Client Connected!!");
                connFrm.connectReq = true;
            }
            catch (Exception ex)
            {
                //ChatListBox.Items.Add("연결 수락 도중 오류 발생 : " + ex.Message);
                return;
            }
            AsyncObject ao = new AsyncObject(4096);
            ao.WorkingSocket = sockClient;
            m_ConnectedClient = sockClient;
            try
            {
                sockClient.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_ReceiveHandler, ao);
            }
            catch (Exception ex)
            {
                //ChatListBox.Items.Add("수신 대기 도중 오류 발생 : " + ex.Message);
                return;
            }
        }

        private void handleReceivedData(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;
            int recvBytes;
            try
            {
                recvBytes = ao.WorkingSocket.EndReceive(ar);
            }
            catch
            {
                return;
            }
            if (recvBytes > 0)
            {
                byte[] msgByte = new byte[recvBytes];
                Array.Copy(ao.Buffer, msgByte, recvBytes);
                //ChatListBox.Items.Add("메세지 받음 : " + Encoding.Unicode.GetString(msgByte));
            }
            try
            {
                ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_ReceiveHandler, ao);
            }
            catch (Exception ex)
            {
                //ChatListBox.Items.Add("수신 도중 오류 발생 : " + ex.Message);
                return;
            }
        }

        private void handleSentData(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;
            int sentBytes;
            try
            {
                sentBytes = ao.WorkingSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                //ChatListBox.Items.Add("전송 도중 오류 발생 : " + ex.Message);
                return;
            }
            if (sentBytes > 0)
            {
                byte[] msgByte = new byte[sentBytes];
                Array.Copy(ao.Buffer, msgByte, sentBytes);
                //ChatListBox.Items.Add("메세지 보냄 : " + Encoding.Unicode.GetString(msgByte));
            }
        }

        private void SendBtn_Click(object sender, EventArgs e)
        {
            //SendMessage(ChatTextBox.Text);
            //ChatTextBox.Text = "";
        }
    }

    public class m_Client
    {
        public class AsyncObject
        {
            public byte[] Buffer;
            public Socket WorkingSocket;
            public AsyncObject(int bufferSize)
            {
                this.Buffer = new byte[bufferSize];
            }
        }
        private Socket m_ClientSocket = null;
        private AsyncCallback m_ReceiveHandler;
        private AsyncCallback m_SendHandler;
        int hostPort;
        string hostName;
        public ConnectorForm connFrm;
        public m_Client(ConnectorForm _connFrm)
        {
            m_ReceiveHandler = new AsyncCallback(handleReceivedData);
            m_SendHandler = new AsyncCallback(handleSentData);
            connFrm = _connFrm;
        }

        public void ConnectToServer()
        {
            hostPort = Int32.Parse(connFrm.textBox1.Text);
            hostName = connFrm.textBox2.Text;
            m_ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            bool isConnected = false;
            // IPEndPoint ep = new IPEndPoint(IPAddress.Parse(hostName), hostPort);
            // MessageBox.Show(ep.ToString());
            try
            {
                m_ClientSocket.Connect(new IPEndPoint(IPAddress.Parse(hostName), hostPort));
                isConnected = true;
            }
            catch
            {
                isConnected = false;
            }
            if (isConnected)
            {
                AsyncObject ao = new AsyncObject(4096);
                ao.WorkingSocket = m_ClientSocket;
                m_ClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_ReceiveHandler, ao);
                connFrm.ConnectLoglistBox.Items.Add("연결 성공");
            }
            else
            {
                connFrm.ConnectLoglistBox.Items.Add("연결 실패");
            }
        }

        private void StopClient()
        {
            m_ClientSocket.Close();
        }

        private void SendMessage(string message)
        {
            AsyncObject ao = new AsyncObject(1);
            ao.Buffer = Encoding.Unicode.GetBytes(message);
            ao.WorkingSocket = m_ClientSocket;
            try
            {
                m_ClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_SendHandler, ao);
            }
            catch (Exception ex)
            {
                connFrm.ConnectLoglistBox.Items.Add("전송 도중 오류 발생 : " + ex.Message);
            }
        }

        private void handleReceivedData(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;
            int recvBytes;
            try
            {
                recvBytes = ao.WorkingSocket.EndReceive(ar);
            }
            catch
            {
                return;
            }
            if (recvBytes > 0)
            {
                byte[] msgByte = new byte[recvBytes];
                Array.Copy(ao.Buffer, msgByte, recvBytes);
                connFrm.ConnectLoglistBox.Items.Add("메세지 받음 : " + Encoding.Unicode.GetString(msgByte));
            }
            try
            {
                ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_ReceiveHandler, ao);
            }
            catch (Exception ex)
            {
                connFrm.ConnectLoglistBox.Items.Add("수신 대기 도중 오류 발생 : " + ex.Message);
                return;
            }
        }

        private void handleSentData(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject)ar.AsyncState;
            int sentBytes;
            try
            {
                sentBytes = ao.WorkingSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                connFrm.ConnectLoglistBox.Items.Add("전송 도중 오류 발생 : " + ex.Message);
                return;
            }
            if (sentBytes > 0)
            {
                byte[] msgByte = new byte[sentBytes];
                Array.Copy(ao.Buffer, msgByte, sentBytes);
                connFrm.ConnectLoglistBox.Items.Add("메세지 보냄 : " + Encoding.Unicode.GetString(msgByte));
            }
        }

        private void DisconnectServerBtn_Click(object sender, EventArgs e)
        {
            StopClient();
        }
    }
}
