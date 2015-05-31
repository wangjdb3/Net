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
        Thread[] workThreads = new Thread[3];
        static object locker = new object();
        static bool[] done = new bool[2];
        public static ThunderAgentLib.AgentClass a = new ThunderAgentLib.AgentClass();
        public static List<string> pic_web_all_url = new List<string>();
        public static List<string> dir = new List<string>();
        public static List<string> pic_web_url_tmp = new List<string>();
        public static List<string> pic_web_url = new List<string>();
        public static List<string> pic_url = new List<string>();

        public static CookieCollection Cookies = null;
        public static string strCookies = string.Empty;

        public static int url_deal_num = 0;

        public bool Web_Login()
        {
            string filedir = Application.StartupPath;
            string username = string.Empty;
            string passwd = string.Empty;
            try
            {
                StreamReader file = new StreamReader(filedir + "\\Net.ini");
                username = file.ReadLine();
                passwd = file.ReadLine();
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("未找到配置文件");
                return false;
            }
            HttpWebResponse response;
            Cookies = new CookieCollection();
            Encoding encoding = Encoding.UTF8;
            string loginUrl = "https://www.latexperiment.com/access/protect/new-rewrite?f=2&url=/subscribers/&host=www.latexperiment.com&ssl=off";
            string html_str = "";
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("amember_login", username);
            parameters.Add("amember_pass", passwd);
            try
            {
                response = HttpWebResponseUtility.CreatePostHttpResponse(loginUrl, parameters, 10000, null, encoding, Cookies);
                html_str = HttpWebResponseUtility.StrGetRequest(response, encoding);
                Cookies = response.Cookies;
                foreach (Cookie cookie in Cookies)
                {
                    strCookies += (cookie.ToString() + "; ");
                }
            }
            catch (WebException ex)
            {
                label1.Text = "登陆状态:" + "登陆超时";
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("登陆发生错误");
                return false;
                //发生其他异常时的处理操作。
            }
            Regex reg = new Regex("Welcome to the subscriber area");
            Match match = reg.Match(html_str);
            if (match.Success)
            {
                label1.Text = "登陆状态:" + "登陆成功";
                button2.Enabled = true;
            }
            else
            {
                label1.Text = "登陆状态:" + "登陆失败";
                return false;
            }
            return true;
        }
        ////////////////////////////////////////////////
        public bool Web_Get()
        {
            string str_html = string.Empty;
            HttpWebResponse response;
            Encoding encoding = Encoding.UTF8;
            string Url = url_text.Text.Trim();
            try
            {
                response = HttpWebResponseUtility.CreateGetHttpResponse(Url, 10000, null, Cookies);
                str_html = HttpWebResponseUtility.StrGetRequest(response, encoding);
            }
            catch (WebException ex)
            {
                MessageBox.Show("访问超时1");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("访问发生错误");
                return false;
                //发生其他异常时的处理操作。
            }
            Web_Url_Get(str_html);
            if (pic_web_all_url.Count != 0)
            {
                Pic_Url_Deal(pic_web_all_url[0], dir[0]);                
            }
            else
            {
                MessageBox.Show("网页有误");
                return false;
            }
            return true;
        }
        ///////////////////////////////////////////////
        public bool Web_Url_Get(string str_html)
        {
            int index = 0;
            Regex reg = new Regex(@"<h2 class=""title""><a href="".*"">.*</a></h2>");
            Match match = reg.Match(str_html, index);
            while (match.Success)
            {
                string Web_Url_tmp = match.Value;
                index = (match.Index + match.Length);
                reg = new Regex(@"https:.*(?="")");
                match = reg.Match(Web_Url_tmp);
                pic_web_all_url.Add(match.Value);
                //pic_web_url.Add(match.Value);
                reg = new Regex(@"(?<=\d"">).*(?=</a>)");
                match = reg.Match(Web_Url_tmp);
                string dir_tmp = @"H:\新建文件夹\LATEX\" + comboBox1.Text + @"\" + match.Value;
                dir.Add(dir_tmp);
                reg = new Regex(@"<h2 class=""title""><a href="".*"">.*</a></h2>");
                match = reg.Match(str_html, index);
            }
            return true;
        }
        ///////////////////////////////////////////////
        public bool Pic_Url_Deal(string web_url, string dir)
        {
            this.Invoke((EventHandler)(delegate
            {
                dir_Text.Text = dir;
            }));
            string str_html = string.Empty;
            HttpWebResponse response;
            Encoding encoding = Encoding.UTF8;
            try
            {
                response = HttpWebResponseUtility.CreateGetHttpResponse(web_url, 10000, null, Cookies);
                str_html = HttpWebResponseUtility.StrGetRequest(response, encoding);
            }
            catch (WebException ex)
            {
                MessageBox.Show("访问超时1");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("访问发生错误");
                return false;
                //发生其他异常时的处理操作。
            }
            Url_Get(str_html);
            if (pic_web_url_tmp.Count != 0)
            {
                this.Invoke((EventHandler)(delegate
                {
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = pic_web_url_tmp.Count;// -1;
                }));
                done[0] = false;
                done[1] = false;
                workThreads[0] = new Thread(new ParameterizedThreadStart(Get));
                workThreads[1] = new Thread(new ParameterizedThreadStart(Get));
                workThreads[2] = new Thread(new ParameterizedThreadStart(Wait));
                workThreads[0].Start(0);
                workThreads[1].Start(1);
                workThreads[2].Start(str_html);
//                t.Join(1000);
/*                for (int i = 0; i < pic_web_url.Count; i++)
                {
                    bool b = Pic_Url_Get(pic_web_url[i]);////////////////////////
                    if (b == false)
                    {
                        progressBar1.Value = 0;
                        label5.Text = "进度：";
                        pic_url.Clear();
                        pic_web_url.Clear();
                        return false;
                    }
                    this.Invoke((EventHandler)(delegate
                    {
                        progressBar1.Value = i;
                    }));
                    this.Invoke((EventHandler)(delegate
                    {
                        label5.Text = "进度：" + (i + 1).ToString() + "/" + pic_web_url.Count;
                    }));
                }*/
            }
            else
            {
                MessageBox.Show("无图片");
                return false;
            }
            return true;
        }
        /// ///////////////////////////////////////////////
        public bool Url_Get(string str_html)
        {
            int index = 0;
            Regex reg = new Regex(@"https://www\.latexperiment\.com/subscribers/\?attachment_id=\d+");
            Match match = reg.Match(str_html, index);
            while (match.Success)
            {
                pic_web_url_tmp.Add(match.Value);
                index = (match.Index + match.Length);
                reg = new Regex(@"https://www\.latexperiment\.com/subscribers/\?attachment_id=\d+");
                match = reg.Match(str_html, index);
            }
            return true;
        }
        ///////////////////////////////////////////////////      
        public bool Pic_Url_Get(string pic_web_url_tmp)
        {
            string str_html = string.Empty;
            int error_num = 0;
            HttpWebResponse response;
            Encoding encoding = Encoding.UTF8;
label:      try
            {
                response = HttpWebResponseUtility.CreateGetHttpResponse(pic_web_url_tmp, 10000, null, Cookies);
                str_html = HttpWebResponseUtility.StrGetRequest(response, encoding);
            }
            catch (WebException ex)
            {
                if (error_num == 5)
                {
                    MessageBox.Show("访问超时2");
                    return false;
                }
                error_num++;
                goto label;
            }
            catch (Exception ex)
            {
                if (error_num == 5)
                {
                    MessageBox.Show("访问发生错误");
                    return false;
                }
                error_num++;
                goto label;
                //发生其他异常时的处理操作。
            }
            Regex reg = new Regex(@"https:.*\.jpg");
            Match match = reg.Match(str_html);
            if (match.Success)
            {
                lock (locker)
                {
                    pic_url.Add(match.Value);
                    pic_web_url.Add(pic_web_url_tmp);
                    url_deal_num++;
                }
            }
            return true;
        }
        public void mp4_url_Get(string str_html)
        {
            Regex reg = new Regex(@"(?<=a href="")https?:.*\.mp4(?="")");
            Match match = reg.Match(str_html);
            if (match.Success)
            {
                pic_web_url.Insert(0, url_text.Text);
                pic_url.Insert(0, match.Value);
            }

        }
        public void th(string strCookie)
        {
            string bstrurl = string.Empty;
            string bstrFileName = "";
            string bstrComments = "";
            string bstrReferUrl = string.Empty;
            int nStartMode = 0;
            int nOnlyFromOrigin = 0;
            int nOriginThreadCount = -1;
            for (int i = 0; i < pic_url.Count; i++)
            {
                bstrurl = pic_url[i];
                bstrReferUrl = pic_web_url[i];
                a.AddTask2(bstrurl, bstrFileName, "", bstrComments, bstrReferUrl, nStartMode, nOnlyFromOrigin, nOriginThreadCount, strCookie);
            }
            a.CommitTasks();
            MessageBox.Show("抓取完成");
        }
        
    }
    
}
