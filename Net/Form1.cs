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
using SendHttp;
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
        }
        CookieContainer cookies = null;
        string strCookies = string.Empty;
        private void button1_Click(object sender, EventArgs e)
        {
            deal.Web_Login(this, ref cookies, ref strCookies);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (url_text.Text == "")
            {
                MessageBox.Show("地址为空");
            }
            else
            {
                deal.Web_Get(this, cookies, strCookies);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deal.th();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            deal.pic_url.Clear();
            deal.pic_web_url.Clear();
            label4.Text = "抓取：";
            url_text.Text = "";
            button3.Enabled = false;
            button4.Enabled = false;
        }
    }
}
