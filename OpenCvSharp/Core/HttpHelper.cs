using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IoTApp.Base.Common.Helpers
{
    public class HttpHelper
    {
        #region Core

        static string[] UserAgents = new string[] {
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; Media Center PC 6.0; Zune 4.0; InfoPath.3; MS-RTC LM 8; .NET4.0C; .NET4.0E)",
            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.133 Safari/534.16",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.57.2 (KHTML, like Gecko) Version/5.1.7 Safari/534.57.2",
            "Mozilla/5.0 (X11; U; Linux x86_64; zh-CN; rv:1.9.2.10) Gecko/20100922 Ubuntu/10.10 (maverick) Firefox/3.6.10",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.80 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; rv:11.0) like Gecko",
        };

        static string AnyUserAgent()
        {
            var index = new Random().Next(0, UserAgents.Length - 1);
            return UserAgents[index];
        }

        static CookieCollection GetDefaultCookies(string url)
        {
            CookieCollection cookies = new CookieCollection();
            Uri uri = new Uri(url);
            string path = "/";
            string domain = uri.Host;
            cookies.Add(new Cookie("_t", DateTime.Now.Ticks.ToString(), path, domain));
            return cookies;
        }

        #endregion

        #region Get

        /// <summary>
        /// 如果异常，则以error开头
        /// </summary>
        /// <param name="url"></param>
        /// <param name="isAddDefaultCookie"></param>
        /// <returns></returns>
        public static string Get(string url, bool isAddDefaultCookie = false)
        {
            CookieCollection cookies = null;
            if (isAddDefaultCookie) cookies = GetDefaultCookies(url);
            return Get(url, 8000, null, null, null, cookies);
        }

        public static string Get(string url, string contentType, NameValueCollection headers)
        {
            return Get(url, 8000, contentType, null, headers, null);
        }

        #endregion

        #region Post

        /// <summary>
        /// 如果异常，则以error开头
        /// </summary>
        /// <returns>服务器返回字符串</returns>
        public static string Post(string url, string parameters, string contentType = null, NameValueCollection headers = null)
        {
            return Post(url, parameters, 8000, contentType, null, headers, null).ResponseString;
        }

        /// <summary>
        /// 如果异常，则以error开头
        /// </summary>
        /// <returns></returns>
        public static HttpResponse Post(string url, string parameters, int timeout, string contentType, Encoding encoding, NameValueCollection headers, CookieCollection cookies)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            byte[] postBytes = encoding.GetBytes(parameters);
            return Post(url, postBytes, timeout, contentType, encoding, headers, cookies);
        }

        #endregion

        #region 核心方法

        /// <summary>
        /// 获得GET方式的HTTP请求后响应的数据
        /// 如果异常，则以error开头
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="timeout">请求的超时时间(以毫秒为单位)</param>
        /// <param name="contentType">设置为"application/x-www-form-urlencoded"或者是"application/json"</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>服务器返回字符串</returns>
        public static string Get(string url, int timeout, string contentType, Encoding encoding, NameValueCollection headers, CookieCollection cookies)
        {
            var responseString = string.Empty;
            HttpWebRequest request = null;
            try
            {
                if (encoding == null) encoding = Encoding.UTF8;
                //创建HttpWebRequest
                request = CreateHttpWebRequest(url, timeout, contentType, encoding, headers, cookies);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (response.ContentEncoding != null && response.ContentEncoding.ToLower().Contains("gzip"))
                        {
                            using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                            {
                                using (StreamReader sr = new StreamReader(stream, encoding))
                                {
                                    responseString = sr.ReadToEnd();
                                }
                            }
                        }
                        else if (response.ContentEncoding != null && response.ContentEncoding.ToLower().Contains("deflate"))
                        {
                            using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                            {
                                using (StreamReader sr = new StreamReader(stream, encoding))
                                {
                                    responseString = sr.ReadToEnd();
                                }
                            }
                        }
                        else
                        {
                            using (Stream stream = response.GetResponseStream())
                            {
                                using (StreamReader sr = new StreamReader(stream, encoding))
                                {
                                    responseString = sr.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                responseString = $"error:{ex.Message}";
            }
            if (request != null)
            {
                request.Abort();
            }
            return responseString;
        }

        /// <summary>
        /// 获得POST方式的HTTP请求后响应的数据
        /// 如果异常，则以error开头
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param>
        /// <param name="timeout">请求的超时时间(以毫秒为单位)</param>
        /// <param name="contentType">设置为"application/x-www-form-urlencoded"或者是"application/json"</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="headers">随同HTTP请求发送的Header信息，如果不需要身份验证可以为空</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns>服务器返回字符串</returns>
        public static HttpResponse Post(string url, byte[] postBytes, int timeout, string contentType, Encoding encoding, NameValueCollection headers, CookieCollection cookies)
        {
            var httpMethod = "POST";
            return GetResponseString(url, httpMethod, postBytes, timeout, contentType, encoding, headers, cookies);
        }

        public static HttpResponse Delete(string url, int timeout, string contentType, Encoding encoding, NameValueCollection headers, CookieCollection cookies)
        {
            var httpMethod = "Delete";
            return GetResponseString(url, httpMethod, null, timeout, contentType, encoding, headers, cookies);
        }

        /// <summary>
        /// 远程获取图片流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="userAgent"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Stream GetResponseStream(string url, int timeout)
        {
            try
            {
                //创建HttpWebRequest
                HttpWebRequest request = CreateHttpWebRequest(url, timeout, null, null, null, null);
                request.Method = "GET";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                return response.GetResponseStream();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="contentType"></param>
        /// <param name="encoding"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpWebRequest(string url, int timeout, string contentType, Encoding encoding, NameValueCollection headers, CookieCollection cookies)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
                //string txtCertFile = "iot3rd.p12";
                //string txtCertPwd = "IoM@1234";

                //X509Certificate certificate = new X509Certificate2(txtCertFile, txtCertPwd);
                //request.ClientCertificates.Add(certificate);

            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            if (timeout > 8000)
            {
                request.Timeout = timeout;
            }
            else
            {
                request.Timeout = 15000;
            }
            if (string.IsNullOrWhiteSpace(contentType))
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            else
            {
                request.ContentType = contentType;
            }
            //request.UserAgent = AnyUserAgent();
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if (headers != null)
            {
                if (request.Headers == null) request.Headers = new WebHeaderCollection();
                request.Headers.Add(headers);
            }
            request.Accept = "*/*";
            //request.KeepAlive = true;
            //request.AllowAutoRedirect = true;

            //WebProxy _proxy = new WebProxy(proxyAddr, Convert.ToInt32(proxyPort));
            //request.Proxy = _proxy;

            return request;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; //总是接受
        }

        public static HttpResponse GetResponseString(string url, string httpMethod, byte[] bodyBytes, int timeout, string contentType, Encoding encoding, NameValueCollection headers, CookieCollection cookies)
        {
            var httpResponse = new HttpResponse();
            HttpWebRequest request = null;
            if (encoding == null) encoding = Encoding.UTF8;
            try
            {
                //创建HttpWebRequest
                request = CreateHttpWebRequest(url, timeout, contentType, encoding, headers, cookies);
                request.Method = httpMethod;
                HttpMethod method = new HttpMethod(httpMethod);
                #region Post
                if (HttpMethod.Post.Equals(method))
                {
                    if (bodyBytes != null)
                    {
                        //方式一
                        //request.ContentLength = postBytes.Length;
                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                        }

                        //方式二
                        ////不需要指定长度,否则报错(在写入所有字节之前不能关闭流。)
                        ////request.ContentLength = myEncode.GetByteCount(param);
                        //using (Stream requestStream = request.GetRequestStream())
                        //{
                        //    using (StreamWriter sw = new StreamWriter(requestStream, encoding))
                        //    {
                        //        sw.Write(parameters);
                        //    }
                        //}
                    }
                }
                #endregion
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    httpResponse.StatusCode = (int)response.StatusCode;
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(responseStream, encoding))
                        {
                            httpResponse.ResponseString = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                httpResponse.ExceptionMessage = ex.Message;
                if (ex.Response != null)
                {
                    using (HttpWebResponse response = ex.Response as HttpWebResponse)
                    {
                        httpResponse.StatusCode = (int)response.StatusCode;
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(responseStream, encoding))
                            {
                                httpResponse.ResponseString = sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                httpResponse.ExceptionMessage = ex.Message;
            }
            if (request != null)
            {
                request.Abort();
            }
            return httpResponse;
        }

        #endregion

        #region PostFile
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters">文本参数</param>
        /// <param name="fileBytesArray">文件字节,支持多个</param>
        /// <param name="timeout"></param>
        /// <param name="encoding"></param>
        /// <param name="headers"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static HttpResponse PostFile(string url, List<FormItemModel> parameters, int timeout, Encoding encoding, NameValueCollection headers, CookieCollection cookies)
        {
            ArrayList requestBytesArray = new ArrayList();
            //multipart/form-data; boundary=--------------------------403050750848908140817154
            string boundary = $"--------------------------{DateTime.Now.Ticks.ToString("x")}";//分隔符
            string contentType = $"multipart/form-data; boundary={boundary}";
            string fileContentType = "application/octet-stream";
            foreach (FormItemModel item in parameters)
            {
                if (item.IsFile)
                {
                    //写入文件内容
                    SetFieldValue(requestBytesArray, boundary, encoding, item.Key, item.FileName, fileContentType, item.FileContent);
                }
                else
                {
                    //写入文本内容
                    SetFieldValue(requestBytesArray, boundary, encoding, item.Key, item.Value);
                }
            }

            byte[] requestBytes = MergeContent(requestBytesArray, boundary, encoding);
            var responseString = Post(url, requestBytes, timeout, contentType, encoding, headers, cookies);
            return responseString;
        }

        /// <summary>
        /// 设置表单文本数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValue">字段值</param>
        /// <returns></returns>
        private static void SetFieldValue(ArrayList requestBytesArray, string boundary, Encoding encoding, String fieldName, String fieldValue)
        {
            //文本数据模板
            //----------------------------096095137437036573662074
            //Content-Disposition: form-data; name="userName"
            //
            //test1
            //上一行需要包含换行
            string httpRow = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n";
            string httpRowData = String.Format(httpRow, boundary, fieldName, fieldValue);
            requestBytesArray.Add(encoding.GetBytes(httpRowData));
        }

        /// <summary>
        /// 设置表单文件数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="fileName">字段值</param>
        /// <param name="fileContentType">内容内型</param>
        /// <param name="fileBytes">文件字节流</param>
        /// <returns></returns>
        private static void SetFieldValue(ArrayList requestBytesArray, string boundary, Encoding encoding, String fieldName, String fileName, String fileContentType, Byte[] fileBytes)
        {
            //文件数据模板
            //----------------------------096095137437036573662074
            //Content-Disposition: form-data; name="file"; filename="1112.png"
            //Content-Type: image/png
            //
            //"文件二进制数据"
            //上一行需要包含换行
            string httpRow = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";
            string httpRowData = String.Format(httpRow, boundary, fieldName, fileName, fileContentType);

            byte[] headerBytes = encoding.GetBytes(httpRowData);
            byte[] endBytes = encoding.GetBytes("\r\n");
            byte[] fileFullBytes = new byte[headerBytes.Length + fileBytes.Length + endBytes.Length];

            headerBytes.CopyTo(fileFullBytes, 0);//写入头部数据
            fileBytes.CopyTo(fileFullBytes, headerBytes.Length);//写入文件数据
            endBytes.CopyTo(fileFullBytes, headerBytes.Length + fileBytes.Length);//写入尾部数据

            requestBytesArray.Add(fileFullBytes);
        }

        /// <summary>
        /// 合并请求数据,使用分隔符隔开
        /// </summary>
        /// <returns></returns>
        private static byte[] MergeContent(ArrayList requestBytesArray, string boundary, Encoding encoding)
        {
            int length = 0;
            int readLength = 0;
            string endBoundary = "--" + boundary + "--\r\n";
            byte[] endBoundaryBytes = encoding.GetBytes(endBoundary);

            requestBytesArray.Add(endBoundaryBytes);

            foreach (byte[] b in requestBytesArray)
            {
                length += b.Length;
            }

            byte[] bytes = new byte[length];

            foreach (byte[] b in requestBytesArray)
            {
                b.CopyTo(bytes, readLength);
                readLength += b.Length;
            }

            return bytes;
        }
        #endregion

        public class HttpResponse
        {
            public int StatusCode { get; set; }
            public string ResponseString { get; set; }
            public string ExceptionMessage { get; set; }
        }

    }

    /// <summary>
    /// 表单数据项
    /// </summary>
    public class FormItemModel
    {
        /// <summary>
        /// 表单键，request["key"]
        /// </summary>
        public string Key { set; get; }
        /// <summary>
        /// 表单值,上传文件时忽略，request["key"].value
        /// </summary>
        public string Value { set; get; }
        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool IsFile
        {
            get
            {
                if (FileContent == null || FileContent.Length == 0 || string.IsNullOrWhiteSpace(FileName))
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName { set; get; }
        /// <summary>
        /// 上传的文件内容
        /// </summary>
        public byte[] FileContent { set; get; }
    }

}
