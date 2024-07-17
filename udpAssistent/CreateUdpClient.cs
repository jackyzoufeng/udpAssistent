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
    public partial class CreateUdpClient : Form
    {
        public string serverip;
        public int port;
        public int localport;
        public CreateUdpClient()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= 0)
            {
                MessageBox.Show($"IP is empty");
                return;
            }
            if (textBox2.Text.Length <= 0)
            {
                MessageBox.Show($"Port is empty");
                return;
            }
            bool isNum1 = Int32.TryParse(textBox2.Text, out port);
            if (!isNum1)
            {
                MessageBox.Show($"{textBox2.Text} is not a digit");
                return;
            }
            if (textBox3.Text.Length > 0)
            {
                bool isNum2 = Int32.TryParse(textBox3.Text, out localport);
                if (!isNum2)
                {
                    MessageBox.Show($"{textBox3.Text} is not a digit");
                    return;
                }
            }
            serverip = textBox1.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
