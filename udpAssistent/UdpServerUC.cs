using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace udpAssistent
{
    public partial class UdpServerUC : UserControl
    {
        UdpManager umm = null;
        public UdpServerUC(UdpManager um)
        {
            InitializeComponent();

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("no", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("ip:port", 200, HorizontalAlignment.Center);
            listView1.Columns.Add("context", 300, HorizontalAlignment.Center);

            um.setContext(WindowsFormsSynchronizationContext.Current, ThreadCallback);
            umm = um;
        }

        public void ThreadCallback(clientData cd)
        {
            ListViewItem item = new ListViewItem((listView1.Items.Count + 1).ToString());
            item.Tag = cd;
            item.SubItems.Add(cd.receiveEndPoint.ToString());
            item.SubItems.Add(Encoding.UTF8.GetString(cd.data));
            listView1.Items.Add(item);
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Windows.Forms.ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                if (items.Count == 1)
                {
                    ContextMenuStrip customMenu = new ContextMenuStrip();
                    customMenu.Items.Add(new ToolStripMenuItem("Send Message", null, new EventHandler(cmsSendMessage)));
                    customMenu.Show(System.Windows.Forms.Cursor.Position);
                }
            }
        }

        private void cmsSendMessage(object sender, EventArgs e)
        {
            System.Windows.Forms.ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
            if (items.Count == 1)
            {
                clientData cd = items[0].Tag as clientData;
                UpdSendMessage usm = new UpdSendMessage();
                if (usm.ShowDialog() == DialogResult.OK)
                {
                    umm.sendmessage(usm.sendmessage, cd.receiveEndPoint);
                }
            }
        }
    }
}
