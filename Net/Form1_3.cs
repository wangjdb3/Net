using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using ThunderAgentLib;
using System.Threading;
using System.Runtime.InteropServices;

namespace Net
{
    public partial class Form1 : Form
    {
        public void Get(object j)
        {
            int i = (int)j;
            while (i < pic_web_url_tmp.Count)
            {
                bool b = Pic_Url_Get(pic_web_url_tmp[i]);////////////////////////
                if (b == false)
                {
                    if (this.progressBar1.InvokeRequired)
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            progressBar1.Value = 0;
                            label5.Text = "进度：";
                            pic_url.Clear();
                            pic_web_url.Clear();
                            pic_web_url_tmp.Clear();
                        }));
                    }
                }
                if (progressBar1.InvokeRequired)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        progressBar1.Value = url_deal_num;
                    }));
                }
                if (label5.InvokeRequired)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        label5.Text = "进度：" + url_deal_num.ToString() + "/" + pic_web_url_tmp.Count;
                    }));
                }
                i += 2;
            }
            done[(int)j] = true;
        }
        public void Wait(object str_html)
        {
            TimeSpan interval = new TimeSpan(0, 0, 5);
            Thread.Sleep(interval);
            while ((done[0] == false) || (done[1] == false)) ;
            mp4_url_Get(str_html.ToString());
            if (pic_url.Count == pic_web_url.Count)
            {
                this.Invoke((EventHandler)(delegate
                {
                    label4.Text = label4.Text + "完成";
                    button3.Enabled = true;
                    button4.Enabled = true;
                }));
                url_deal_num = 0;
                pic_web_all_url.RemoveAt(0);
                dir.RemoveAt(0);
            }
            else
            {
                pic_url.Clear();
                pic_web_url.Clear();
                pic_web_url_tmp.Clear();
                MessageBox.Show("数据不匹配");
            }
        }
    }
}
