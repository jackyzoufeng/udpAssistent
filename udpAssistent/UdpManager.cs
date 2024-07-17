using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udpAssistent
{
    public class UdpManager
    {
        UdpClient udpServer;
        public int udpPort;
        Thread rec;
        bool stopThread = true;
        SynchronizationContext context = null;
        System.Action<clientData> callback;
        List<clientData> clients = new List<clientData>();
        public UdpManager()
        {
            
        }
        public bool start(int port)
        {
            bool ret = true;
            try
            {
                udpServer = new UdpClient(port);
                udpPort = port;
                stopThread = false;
                rec = new Thread(new ThreadStart(this.receive));
                rec.Start();
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
            udpServer.Close();
            rec.Join();
            Console.WriteLine($"{this} was stopped");
        }

        public void setContext(SynchronizationContext cxt, System.Action<clientData> cb)
        {
            context = cxt;
            callback = cb;
            foreach (clientData data1 in clients) 
            { 
                context.Post(new SendOrPostCallback((o) => 
                { 
                    callback((clientData)o); 
                }), data1);
            }
        }

        public void sendmessage(string msg, IPEndPoint ep)
        {
            byte[] bary = Encoding.UTF8.GetBytes(msg);
            udpServer.Send(bary, bary.Length, ep);
        }

        private void receive()
        {
            IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = new byte[1024];
            while (!stopThread)
            {
                try
                {
                    data = udpServer.Receive(ref receiveEndPoint);
                    clientData data1 = new clientData();
                    data1.receiveEndPoint = receiveEndPoint;
                    data1.data = data;
                    clients.Add(data1);
                    if (context != null)
                    {
                        context.Post(new SendOrPostCallback((o) => 
                        { 
                            callback((clientData)o); 
                        }), data1);
                    }
                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine($"receive data `{message}` from `{receiveEndPoint}`");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
    public class clientData
    {
        public IPEndPoint receiveEndPoint;
        public byte[] data;
    }
}
