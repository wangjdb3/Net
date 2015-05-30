using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Net
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            checkBox1.Enabled = false;
        }
        CookieCollection Cookies = null;
        string strCookies = string.Empty;
        private void button1_Click(object sender, EventArgs e)
        {
            deal.Web_Login(this, ref Cookies, ref strCookies);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (url_text.Text == "")
            {
                MessageBox.Show("地址为空");
            }
            else
            {
                deal.Web_Get(this, Cookies, strCookies);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deal.th();
            checkBox1.Checked = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            deal.pic_url.Clear();
            deal.pic_web_url.Clear();
            label4.Text = "抓取：";
            label5.Text = "进度：";
            url_text.Text = "";
            button3.Enabled = false;
            button4.Enabled = false;
            checkBox1.Checked = false;
            progressBar1.Value = 0;
        }
    }
}
