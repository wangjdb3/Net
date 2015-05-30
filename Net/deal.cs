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
    class deal
    {
        public static ThunderAgentLib.AgentClass a = new ThunderAgentLib.AgentClass();

        public static List<string> pic_web_url = new List<string>();
        public static List<string> pic_url = new List<string>();

        public static string bstrPath = "";
        public static string strCookie = "";
        public static bool Web_Login(Form1 f, ref CookieCollection cookies, ref string strCookies)
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
            CookieCollection Cookies = new CookieCollection();
            Encoding encoding = Encoding.UTF8;
            string loginUrl = "https://www.latexperiment.com/access/protect/new-rewrite?f=2&url=/subscribers/&host=www.latexperiment.com&ssl=off";
            string html_str = "";
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("amember_login", username);
            parameters.Add("amember_pass", passwd);
            string cookieString = "";
            try
            {
                response = HttpWebResponseUtility.CreatePostHttpResponse(loginUrl, parameters, 10000, null, encoding, Cookies);
                html_str = HttpWebResponseUtility.StrGetRequest(response, encoding);
                Cookies = response.Cookies;
                foreach (Cookie cookie in Cookies)
                {
                    cookieString += (cookie.ToString() + "; ");
                }
            }
            catch (WebException ex)
            {
                f.label1.Text = "登陆状态:" + "登陆超时";
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
                f.label1.Text = "登陆状态:" + "登陆成功";
                f.button2.Enabled = true;
            }
            else
            {
                f.label1.Text = "登陆状态:" + "登陆失败";
                return false;
            }
            cookies = Cookies;
            strCookies = cookieString;
            return true;
        }
        ////////////////////////////////////////////////
        public static bool Web_Get(Form1 f, CookieCollection Cookies, string strCookies)
        {
            string str_html = string.Empty;
            HttpWebResponse response;
            Encoding encoding = Encoding.UTF8;
            bstrPath = f.dir_Text.Text.Trim();
            string Url = f.url_text.Text.Trim();
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
            Url_Get(f,ref pic_web_url, str_html);
            if (pic_web_url.Count != 0)
            {
                f.Invoke((EventHandler)(delegate
                {
                    f.progressBar1.Maximum = pic_web_url.Count - 1;
                }));
                for (int i = 0; i < pic_web_url.Count; i++)
                {
                    bool b = Pic_Url_Get(pic_web_url[i], Cookies, strCookies, ref pic_url);////////////////////////
                    if (b == false)
                    {
                        f.progressBar1.Value = 0;
                        f.label5.Text = "进度：";
                        pic_url.Clear();
                        pic_web_url.Clear();
                        return false;
                    }
                    f.Invoke((EventHandler)(delegate
                    {
                        f.progressBar1.Value = i;
                        f.label5.Text = "进度：" + (i + 1).ToString() + "/" + pic_web_url.Count;
                    }));
                }
                mp4_url_Get(f, ref pic_web_url, str_html, ref pic_url);
                if (pic_url.Count == pic_web_url.Count)
                {
                    f.label4.Text = f.label4.Text + "完成";
                    f.button3.Enabled = true;
                    f.button4.Enabled = true;
                }
                else
                {
                    MessageBox.Show("数据不匹配");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("无图片");
                return false;
            }
            return true;
        }
        ///////////////////////////////////////////////
        public static bool Url_Get(Form1 f,ref List<string> pic_web_url, string str_html)
        {
            int index = 0;
            Regex reg = new Regex(@"https://www\.latexperiment\.com/subscribers/\?attachment_id=\d+");
            Match match = reg.Match(str_html, index);
            while (match.Success)
            {
                pic_web_url.Add(match.Value);
                index = (match.Index + match.Length);
                reg = new Regex(@"https://www\.latexperiment\.com/subscribers/\?attachment_id=\d+");
                match = reg.Match(str_html, index);
            }
            return true;
        }
        ///////////////////////////////////////////////////        
        public static bool Pic_Url_Get(string pic_web_url, CookieCollection Cookies, string strCookies, ref List<string> pic_url)
        {
            string str_html = string.Empty;
            int error_num = 0;
            HttpWebResponse response;
            Encoding encoding = Encoding.UTF8;
label:      try
            {
                response = HttpWebResponseUtility.CreateGetHttpResponse(pic_web_url, 10000, null, Cookies);
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
                pic_url.Add(match.Value);
            }
            return true;
        }
        public static void mp4_url_Get(Form1 f, ref List<string> pic_web_url, string str_html, ref List<string> pic_url)
        {
            Regex reg = new Regex(@"(?<=a href="")https?:.*\.mp4(?="")");
            Match match = reg.Match(str_html);
            if (match.Success)
            {
                pic_web_url.Insert(0, f.url_text.Text);
                pic_url.Insert(0, match.Value);
            }

        }
        public static void th()
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
                a.AddTask2(bstrurl, bstrFileName, bstrPath, bstrComments, bstrReferUrl, nStartMode, nOnlyFromOrigin, nOriginThreadCount, strCookie);
            }
            a.CommitTasks();
            MessageBox.Show("抓取完成");
        }
    }
    
}
