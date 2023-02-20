using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Apotheosis
{
    public class ApothClient
    {
        IPEndPoint remoteEndPoint;
        UdpClient client;
        public bool isListening = false;
        public bool isLoggedIn = false;
        public ApothClient(IPEndPoint server)
        {
            client = new UdpClient();
            remoteEndPoint = server;
        }

        public async void Connect()
        {
            client.Connect(remoteEndPoint);
            isListening = true;
            await Task.Run(BeginListening);
        }

        public async Task BeginListening()
        {
            while (true)
            {
                var data = await client.ReceiveAsync();
                OnUdpData(data);
            }
        }

        private void SendBytes(byte[] bytes)
        {
            if(client != null)
            {
                client.SendAsync(bytes, bytes.Length);
            }
        }

        public void Message(string message)
        {
            SendBytes(Encoding.UTF8.GetBytes(message));
        }

        void OnUdpData(UdpReceiveResult result)
        {
            //this is where we handle incoming data from any connection
            var senderIp = result.RemoteEndPoint;
            var data = result.Buffer;
            if (senderIp == remoteEndPoint)
            {
                if (data != null)
                {
                    Console.WriteLine($"|{DateTime.Now.ToShortTimeString()}|{result.RemoteEndPoint}:{Encoding.UTF8.GetString(result.Buffer)}");
                }
                else
                {
                    //data was null
                }
            }
            else
            {
                //this was data sent from someone who is not the server
                //since we are a client, we really don't want that
            }
        }
    }
}
