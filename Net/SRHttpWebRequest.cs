using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SendHttp
{
    class SRHttpWebRequest
    {
        private HttpWebRequest webRequest;
        private HttpWebResponse webResponse;
        private CookieContainer cookie;
        private List<string> parameter;

        public SRHttpWebRequest()
        {
            cookie = new CookieContainer();
            parameter = new List<string>();
        }

        /// <summary>
        /// 创建webRequest
        /// </summary>
        /// <param name="url">请求地址</param>
        public void Create(string url)
        {
            webRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            webRequest.CookieContainer = cookie;
            parameter.Clear();
        }

        public void AddParameter(string key, string value)
        {
            parameter.Add(key + "=" + value);
        }

        public string RunGetRequest()
        {
            return RunGetRequest(Encoding.UTF8);
        }
        /// <summary>
        /// 以get方式获得回应
        /// </summary>
        /// <param name="encoding">编码方式</param>
        /// <returns>结果字符串</returns>
        public string RunGetRequest(Encoding encoding)
        {
            webRequest.Method = "GET";
            webResponse = (HttpWebResponse)webRequest.GetResponse();
            Stream newStream = webResponse.GetResponseStream();
            StreamReader sr = new StreamReader(newStream, encoding);
            try
            {
                string s = sr.ReadToEnd();
                return s;
            }
            finally
            {
                newStream.Close();
                sr.Close();
            }
        }
        public Stream RunGetRequestStream()
        {
            webRequest.Method = "GET";
            webResponse = (HttpWebResponse)webRequest.GetResponse();
            Stream newStream = webResponse.GetResponseStream();
            return newStream;
        }

        public string WritePostAndGetResponse()
        {
            return WritePostAndGetResponse(Encoding.UTF8);
        }

        /// <summary>
        /// 以post方式获得回应
        /// </summary>
        /// <param name="encoding">编码方式</param>
        /// <returns>结果字符串</returns>
        public string WritePostAndGetResponse(Encoding encoding)
        {
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] postData = GetParameterPostData();
            using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(postData, 0, postData.Length);
                newStream.Flush();
                newStream.Close();
            }
            webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), encoding);
            try
            {
                string s = sr.ReadToEnd();
                return s;
            }
            finally
            {
                sr.Close();
            }
        }

        public string WritePostAndGetResponse(Encoding encoding, string StoodArguments, string ContentType)
        {
            webRequest.Method = "POST";
            webRequest.ContentType = ContentType;
            Stream newStream = webRequest.GetRequestStream();
            byte[] postData = Encoding.UTF8.GetBytes(StoodArguments);
            newStream.Write(postData, 0, postData.Length);
            newStream.Flush();
            newStream.Close();
            webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), encoding);
            try
            {
                string s = sr.ReadToEnd();
                return s;
            }
            finally
            {
                sr.Close();
            }
        }

        public string WritePostAndGetResponse(Encoding encoding, string StoodArguments, string ContentType, Encoding e)
        {
            webRequest.Method = "POST";
            webRequest.ContentType = ContentType;
            Stream newStream = webRequest.GetRequestStream();
            byte[] postData = e.GetBytes(StoodArguments);
            newStream.Write(postData, 0, postData.Length);
            newStream.Flush();
            newStream.Close();
            webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), encoding);
            try
            {
                string s = sr.ReadToEnd();
                return s;
            }
            finally
            {
                sr.Close();
            }
        }

        private byte[] GetParameterPostData()
        {
            string s = string.Join("&", parameter.ToArray());
            parameter.Clear();
            return System.Text.Encoding.ASCII.GetBytes(s);
        }

        public CookieContainer GetRequestCookie()
        {
            return cookie;
        }
        public string Refer
        {
            get { return webRequest.Referer; }
            set { webRequest.Referer = value; }
        }
        public WebHeaderCollection Header
        {
            get { return webRequest.Headers; }
            set { webRequest.Headers = value; }
        }
        public HttpWebRequest WebRequest
        {
            get { return webRequest; }
            set { webRequest = value; }
        }
        public HttpWebResponse WebResponse
        {
            get { return webResponse; }
        }
        public CookieContainer Cookie
        {
            get { return cookie; }
            set { cookie = value; }
        }
        public HttpWebRequest HWR
        {
            get { return this.webRequest; }
        }
    }
}
