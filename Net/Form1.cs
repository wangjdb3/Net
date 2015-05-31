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
        
        private void button1_Click(object sender, EventArgs e)
        {
            Web_Login();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (url_text.Text == "")
            {
                MessageBox.Show("地址为空");
            }
            else
            {
                Web_Get();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dir_Text.Text);
            th(strCookies);
            checkBox1.Checked = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pic_url.Clear();
            pic_web_url.Clear();
            pic_web_url_tmp.Clear();
            this.Invoke((EventHandler)(delegate
            {
                label4.Text = "抓取：";
                label5.Text = "进度：";
                dir_Text.Text = "";
                button3.Enabled = false;
                button4.Enabled = false;
                checkBox1.Checked = false;
                progressBar1.Value = 0;
            }));
            if(pic_web_all_url.Count!=0)
            {
                Pic_Url_Deal(pic_web_all_url[0], dir[0]);
            }
            else
            {
                MessageBox.Show("完成");
            }
        }
    }
}
