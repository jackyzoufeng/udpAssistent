using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace udpAssistent
{
    public enum UDP_TYPE
    {
        UDP_TYPE_SERVER,
        UDP_TYPE_CLIENT
    }
    public partial class Form1 : Form
    {
        ContextMenuStrip udpServerMS = new ContextMenuStrip();
        ContextMenuStrip udpClientMS = new ContextMenuStrip();

        TreeNode treeNodeUdpServer = new TreeNode("UDP Server");
        TreeNode treeNodeUdpClient = new TreeNode("UDP Client");

        ContextMenuStrip nodeMenu = new ContextMenuStrip();

        public Form1()
        {
            InitializeComponent();

            ToolStripMenuItem SMSCreate = new ToolStripMenuItem();
            SMSCreate.Text = "Create Service";
            SMSCreate.Click += SMSCreate_Click;
            udpServerMS.Items.Add(SMSCreate);

            ToolStripMenuItem CMSCreate = new ToolStripMenuItem();
            CMSCreate.Text = "Create Client";
            CMSCreate.Click += CMSCreate_Click;
            udpClientMS.Items.Add(CMSCreate);

            ToolStripMenuItem closeServer = new ToolStripMenuItem();
            closeServer.Text = "Close";
            closeServer.Click += CloseServer_Click;
            nodeMenu.Items.Add(closeServer);

            treeNodeUdpServer.Tag = UDP_TYPE.UDP_TYPE_SERVER;
            treeNodeUdpServer.ContextMenuStrip = udpServerMS;
            treeView1.Nodes.Add(treeNodeUdpServer);

            treeNodeUdpClient.Tag = UDP_TYPE.UDP_TYPE_CLIENT;
            treeNodeUdpClient.ContextMenuStrip = udpClientMS;
            treeView1.Nodes.Add(treeNodeUdpClient);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                if (CurrentNode != null)
                {
                    treeView1.SelectedNode = CurrentNode;//选中这个节点
                }
            }
        }

        private void SMSCreate_Click(object sender, EventArgs e)
        {
            CreateUdpServer createUdpServer = new CreateUdpServer();
            if (createUdpServer.ShowDialog() == DialogResult.OK)
            {
                UdpManager um = new UdpManager();
                if (um.start(createUdpServer.port))
                {
                    TreeNode server1 = new TreeNode($"port:{createUdpServer.port}");
                    server1.ContextMenuStrip = nodeMenu;
                    server1.Tag = um;
                    treeNodeUdpServer.Nodes.Add(server1);
                    treeNodeUdpServer.Expand();
                }
            }
        }

        private void CMSCreate_Click(object sender, EventArgs e)
        {
            CreateUdpClient createUdpClient = new CreateUdpClient();
            if (createUdpClient.ShowDialog() == DialogResult.OK)
            {
                UdpClientManager ucm = new UdpClientManager();
                if (ucm.start(createUdpClient.serverip, createUdpClient.port, createUdpClient.localport))
                {
                    TreeNode client1 = new TreeNode($"{createUdpClient.serverip}:{createUdpClient.port}");
                    client1.ContextMenuStrip = nodeMenu;
                    client1.Tag = ucm;
                    treeNodeUdpClient.Nodes.Add(client1);
                    treeNodeUdpClient.Expand();
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode CurrentNode = e.Node;
            if (CurrentNode != null)
            {
                if (CurrentNode.Tag.GetType() == typeof(UDP_TYPE))
                {
                    Console.WriteLine("UDP_TYPE");
                }
                else if (CurrentNode.Tag.GetType() == typeof(UdpManager))
                {
                    bool found = false;
                    UdpManager um = (UdpManager)CurrentNode.Tag;
                    foreach (TabPage page in tabControl1.TabPages)
                    {
                        if (um == page.Tag)
                        {
                            tabControl1.SelectTab(page);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        TabPage udp_manager_page = new TabPage($"port:{um.udpPort}");
                        udp_manager_page.Tag = um;
                        udp_manager_page.Dock = DockStyle.Fill;
                        UdpServerUC usuc = new UdpServerUC(um);
                        usuc.Dock = DockStyle.Fill;
                        usuc.Parent = udp_manager_page;
                        tabControl1.TabPages.Add(udp_manager_page);
                        tabControl1.SelectTab(udp_manager_page);
                    }
                }
                else if (CurrentNode.Tag.GetType() == typeof(UdpClientManager))
                {
                    bool found = false;
                    UdpClientManager ucm = (UdpClientManager)CurrentNode.Tag;
                    foreach (TabPage page in tabControl1.TabPages)
                    {
                        if (ucm == page.Tag)
                        {
                            tabControl1.SelectTab(page);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        TabPage udp_client_manager_page = new TabPage($"{ucm.serverip}:{ucm.serverport}");
                        udp_client_manager_page.Tag = ucm;
                        udp_client_manager_page.Dock = DockStyle.Fill;
                        UdpClientUC ucuc = new UdpClientUC(ucm);
                        ucuc.Dock = DockStyle.Fill;
                        ucuc.Parent = udp_client_manager_page;
                        tabControl1.TabPages.Add(udp_client_manager_page);
                        tabControl1.SelectTab(udp_client_manager_page);
                    }
                }
            }
        }

        private void CloseServer_Click(object sender, EventArgs e)
        {
            TreeNode CurrentNode = treeView1.SelectedNode;
            CloseNode(CurrentNode);
        }

        private void CloseNode(TreeNode node)
        {
            if (node.Tag.GetType() == typeof(UdpManager))
            {
                UdpManager um = (UdpManager)node.Tag;
                um.stop();

                treeNodeUdpServer.Nodes.Remove(node);
                foreach (TabPage page in tabControl1.TabPages)
                {
                    if (um == page.Tag)
                    {
                        tabControl1.TabPages.Remove(page);
                    }
                }
            }
            else if (node.Tag.GetType() == typeof(UdpClientManager))
            {
                UdpClientManager ucm = (UdpClientManager)node.Tag;
                ucm.stop();

                treeNodeUdpServer.Nodes.Remove(node);
                foreach (TabPage page in tabControl1.TabPages)
                {
                    if (ucm == page.Tag)
                    {
                        tabControl1.TabPages.Remove(page);
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (TreeNode item in treeNodeUdpServer.Nodes)
            {
                CloseNode(item);
            }
            foreach (TreeNode item in treeNodeUdpClient.Nodes)
            {
                CloseNode(item);
            }
        }
    }
}
