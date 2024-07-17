using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udpAssistent
{
    public partial class UdpClientUC : UserControl
    {
        UdpClientManager ucmm = null;
        public UdpClientUC(UdpClientManager ucm)
        {
            InitializeComponent();

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("no", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("context", 300, HorizontalAlignment.Center);

            ucm.setContext(WindowsFormsSynchronizationContext.Current, ThreadCallback, LocalPortCallback);
            ucmm = ucm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                ucmm.sendmessage(textBox1.Text);
            }
        }

        public void ThreadCallback(recvData cd)
        {
            ListViewItem item = new ListViewItem((listView1.Items.Count + 1).ToString());
            item.Tag = cd;
            item.SubItems.Add(Encoding.UTF8.GetString(cd.data));
            listView1.Items.Add(item);
        }

        public void LocalPortCallback(int localport)
        {
            label2.Text = localport.ToString();
        }
    }
}
