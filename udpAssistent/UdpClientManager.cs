using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udpAssistent
{
    public class UdpClientManager
    {
        UdpClient udpClient;
        Thread rec = null;
        bool stopThread = true;
        public string serverip;
        public int serverport;
        IPEndPoint serverEndPoint;
        SynchronizationContext context = null;
        System.Action<recvData> callback;
        System.Action<int> callbacki;
        List<recvData> recvs = new List<recvData>();
        public UdpClientManager()
        {

        }

        public bool start(string ip, int port, int localport)
        {
            bool ret = true;
            try
            {
                if (localport == 0)
                {
                    udpClient = new UdpClient();
                }
                else
                {
                    udpClient = new UdpClient(localport);
                }
                serverip = ip;
                serverport = port;
                serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ret = false;
            }

            return ret;
        }

        public void stop()
        {
            stopThread = true;
            udpClient.Close();
            rec.Join();
            Console.WriteLine($"{this} was stopped");
        }

        public void setContext(SynchronizationContext cxt, System.Action<recvData> cb, System.Action<int> ci)
        {
            context = cxt;
            callback = cb;
            callbacki = ci;
            foreach (recvData data1 in recvs)
            {
                context.Post(new SendOrPostCallback((o) =>
                {
                    callback((recvData)o);
                }), data1);
            }
        }

        public void sendmessage(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            int send = udpClient.Send(data, data.Length, serverEndPoint);
            if (context != null)
            {
                context.Post(new SendOrPostCallback((o) =>
                {
                    callbacki((int)o);
                }), ((System.Net.IPEndPoint)udpClient.Client.LocalEndPoint).Port);
            }
            Console.WriteLine($"send length:{send}");
            if (rec == null)
            {
                startRecvThread();
            }
        }

        private void startRecvThread()
        {
            stopThread = false;
            rec = new Thread(new ThreadStart(this.receive));
            rec.Start();
        }

        private void receive()
        {
            byte[] data = new byte[1024];
            while (!stopThread)
            {
                try
                {
                    data = udpClient.Receive(ref serverEndPoint);
                    recvData data1 = new recvData();
                    data1.data = data;
                    recvs.Add(data1);
                    if (context != null)
                    {
                        context.Post(new SendOrPostCallback((o) =>
                        {
                            callback((recvData)o);
                        }), data1);
                        context.Post(new SendOrPostCallback((o) =>
                        {
                            callbacki((int)o);
                        }), ((System.Net.IPEndPoint)udpClient.Client.LocalEndPoint).Port);
                    }
                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine($"receive data `{message}` from `{serverEndPoint}`");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }

    public class recvData
    {
        public byte[] data;
    }
}
