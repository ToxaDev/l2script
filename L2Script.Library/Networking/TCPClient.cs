using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace L2Script.Library.Networking
{
    public abstract class TCPClient
    {
        private byte[] buffer = new byte[0];
        private Thread listenThread;
        public Socket Client;
        private IPEndPoint _ep;
        public bool Connected = false;

        public abstract void DataReceived(ref byte[] packet);

        public void Connect(string address, ushort port)
        {
            try
            {
                IPAddress[] ip = Dns.GetHostAddresses(address);
                if (ip.Length > 0) this._ep = new IPEndPoint(ip[0], (int)port);
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client.Connect(this._ep);
                this.listenThread = new Thread(new ThreadStart(TCPListen));
                this.listenThread.Start();
            }
            catch (Exception ex)
            {
                Debug.Exception("Unable to connect to server.", ex);
            }
        }

        public void Send(byte[] Packet)
        {
            Client.Send(Packet);
        }

        private void TCPListen()
        {
            Connected = Client.Connected;
            while (Connected)
            {
                try
                {
                    byte[] tmpbuffer = new byte[1];
                    Client.Receive(tmpbuffer, 1, SocketFlags.None);
                    DataArrival(tmpbuffer[0]);
                }
                catch (Exception ex)
                {
                    Debug.Exception("Error getting/processing data from server.", ex);
                }
            }
        }

        public void Close()
        {
            Client.Shutdown(SocketShutdown.Both);
            Connected = Client.Connected;
        }

        public void DataArrival(byte inppacket)
        {
            byte[] newBuffer = new byte[buffer.Length + 1];

            int position = 0;

            for (int i = 0; i < buffer.Length; i++)
                newBuffer[position++] = buffer[i];

            newBuffer[position++] = inppacket;
            int packetLength = -1;
            try
            {
                packetLength = BitConverter.ToInt16(new byte[] { newBuffer[0], newBuffer[1] }, 0);
            }
            catch (Exception) { }
            if (newBuffer.Length >= packetLength && packetLength != -1)
            {
                int offset2 = 0;
                byte[] finalPacket = new byte[packetLength];
                byte[] leftOver = new byte[newBuffer.Length - packetLength];
                for (int x = 0; x < finalPacket.Length; x++)
                    finalPacket[x] = newBuffer[offset2++];

                for (int x = 0; x < leftOver.Length; x++)
                    leftOver[x] = newBuffer[offset2++];

                newBuffer = leftOver;
                DataReceived(ref finalPacket);
            }
            buffer = newBuffer;
        }
    }
}
