using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace OpenCvSharp.ExtendApi
{
    public class FacePlusPlusApiHelper
    {
        private static HttpClient _httpClient;
        //private static readonly AliyunSmsSetting _setting;

        static FacePlusPlusApiHelper()
        {
            //_setting = ExtendApiSettings.GetAppSettings<AliyunSmsSetting>("AliyunSmsSetting");
            //_setting = new AliyunSmsSetting
            //{
            //    Host = "https://api-cn.faceplusplus.com/facepp/v3/detect",
            //    AccessKeyId = "UKClbXP9uWUP-qWYz77oc_6rFUogj2sc",
            //    AccessKeySecret = "ydpVgEBTBTSAmJCBukYiPjkr1cOpNiEp"
            //};
            SetHttpClient();
        }
        static void SetHttpClient()
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri(_setting.Host) };
            //_httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

            //httpclientHandler.Proxy = null;
            //httpclientHandler.AutomaticDecompression = DecompressionMethods.GZip;
            //_httpClient.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * 100000);
            try
            {
                //帮HttpClient热身
                _httpClient.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Head,
                    RequestUri = new Uri(_setting.Host + "/")
                }).Result.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
            }
        }
        private static string GetResponseString(HttpWebResponse httpResponse)
        {
            string responseString = null;


            using (Stream responseStream = httpResponse.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(responseStream, Encoding.UTF8))
                {
                    responseString = sr.ReadToEnd();
                }
            }

            return responseString;
        }

        public static CommonExtendApiResult Detect()
        {
            var result = new CommonExtendApiResult();

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("return_landmark", "1");
            verifyPostParameters.Add("return_attributes", "gender,age,smiling,headpose,facequality,blur,eyestatus,emotion,ethnicity,beauty,mouthstatus,eyegaze,skinstatus");

            byte[] fileImage;
            using (Stream stream = File.Open("tmp/1.jpg", FileMode.Open))// 图片地址
            {
                fileImage = new byte[stream.Length];
                stream.Read(fileImage, 0, fileImage.Length);
                // 设置当前流的位置为流的开始
                stream.Seek(0, SeekOrigin.Begin);
            }

            //添加图片参数
            verifyPostParameters.Add("image_file", new HttpHelper4MultipartForm.FileParameter(fileImage, "1.jpg", "application/octet-stream"));
            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost("https://api-cn.faceplusplus.com/facepp/v3/detect", "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);


            return result;
        }


        public static CommonExtendApiResult CreateFaceSet()
        {
            var result = new CommonExtendApiResult();
            string apiPath = $"https://api-cn.faceplusplus.com/facepp/v3/faceset/create";

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("display_name", "demoFaceSet");
            verifyPostParameters.Add("face_tokens", "cdd88d7f918f8d1a7757a37fd42c0adc,25e0cfd15ec6abb3d5ece864f2b927f2,72f2f7fa47cd29b6a09d61ee079d8bd7");

            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
            //"faceset_token":"1da59b4c728cadb43092bfa0cdfa4b76",
        }

        public static CommonExtendApiResult AddFace()
        {
            var result = new CommonExtendApiResult();
            string apiPath = $"https://api-cn.faceplusplus.com/facepp/v3/faceset/addface";

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("faceset_token", "1da59b4c728cadb43092bfa0cdfa4b76");
            //verifyPostParameters.Add("face_tokens", "cdd88d7f918f8d1a7757a37fd42c0adc,25e0cfd15ec6abb3d5ece864f2b927f2,72f2f7fa47cd29b6a09d61ee079d8bd7");
            verifyPostParameters.Add("face_tokens", "16e0bd620a10929b330f7abf28027267");
            //16e0bd620a10929b330f7abf28027267
            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
            //"faceset_token":"1da59b4c728cadb43092bfa0cdfa4b76",
        }
        public static CommonExtendApiResult Search()
        {
            var result = new CommonExtendApiResult();
            string apiPath = $"https://api-cn.faceplusplus.com/facepp/v3/search";
            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("faceset_token", "1da59b4c728cadb43092bfa0cdfa4b76");

            byte[] fileImage;
            using (Stream stream = File.Open("tmp/3.jpg", FileMode.Open))// 图片地址
            {
                fileImage = new byte[stream.Length];
                stream.Read(fileImage, 0, fileImage.Length);
                // 设置当前流的位置为流的开始
                stream.Seek(0, SeekOrigin.Begin);
            }

            //添加图片参数
            verifyPostParameters.Add("image_file", new HttpHelper4MultipartForm.FileParameter(fileImage, "1.jpg", "application/octet-stream"));
            HttpWebResponse httpResponse = HttpHelper4MultipartForm.MultipartFormDataPost(apiPath, "", verifyPostParameters);
            string responseString = GetResponseString(httpResponse);

            return result;
            //a12e6b8330b1a65a5f211f6793691b22
        }

    }


    public static class HttpHelper4MultipartForm
    {
        public class FileParameter
        {
            public byte[] File
            {
                get;
                set;
            }

            public string FileName
            {
                get;
                set;
            }

            public string ContentType
            {
                get;
                set;
            }

            public FileParameter(byte[] file) : this(file, null)
            {
            }

            public FileParameter(byte[] file, string filename) : this(file, filename, null)
            {
            }

            public FileParameter(byte[] file, string filename, string contenttype)
            {
                this.File = file;
                this.FileName = filename;
                this.ContentType = contenttype;
            }
        }
        private static readonly Encoding encoding = Encoding.UTF8;
        /// <summary>
        /// MultipartForm请求
        /// </summary>
        /// <param name="postUrl">服务地址</param>
        /// <param name="userAgent"></param>
        /// <param name="postParameters">参数</param>
        /// <returns></returns>
        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string text = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + text;//multipart类型
            byte[] multipartFormData = HttpHelper4MultipartForm.GetMultipartFormData(postParameters, text);
            return HttpHelper4MultipartForm.PostForm(postUrl, userAgent, contentType, multipartFormData);
        }

        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(postUrl) as HttpWebRequest;
            if (httpWebRequest == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            httpWebRequest.Method = "POST";//post方式
            httpWebRequest.SendChunked = false;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Proxy = null;
            httpWebRequest.Timeout = Timeout.Infinite;
            httpWebRequest.ReadWriteTimeout = Timeout.Infinite;
            httpWebRequest.AllowWriteStreamBuffering = false;
            httpWebRequest.ProtocolVersion = HttpVersion.Version11;
            httpWebRequest.ContentType = contentType;
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.ContentLength = (long)formData.Length;

            try
            {
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    int bufferSize = 4096;
                    int position = 0;
                    while (position < formData.Length)
                    {
                        bufferSize = Math.Min(bufferSize, formData.Length - position);
                        byte[] data = new byte[bufferSize];
                        Array.Copy(formData, position, data, 0, bufferSize);
                        requestStream.Write(data, 0, data.Length);
                        position += data.Length;
                    }
                    requestStream.Close();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            HttpWebResponse result;
            try
            {
                result = (httpWebRequest.GetResponse() as HttpWebResponse);
            }
            catch (WebException arg_9C_0)
            {
                result = (arg_9C_0.Response as HttpWebResponse);
            }
            return result;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        /// <summary>
        /// 从表单中获取数据
        /// </summary>
        /// <param name="postParameters"></param>
        /// <param name="boundary"></param>
        /// <returns></returns>
        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream stream = new MemoryStream();
            bool flag = false;
            foreach (KeyValuePair<string, object> current in postParameters)
            {
                if (flag)
                {
                    stream.Write(HttpHelper4MultipartForm.encoding.GetBytes("\r\n"), 0, HttpHelper4MultipartForm.encoding.GetByteCount("\r\n"));
                }
                flag = true;
                if (current.Value is HttpHelper4MultipartForm.FileParameter)
                {
                    HttpHelper4MultipartForm.FileParameter fileParameter = (HttpHelper4MultipartForm.FileParameter)current.Value;
                    string s = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n", new object[]
                    {
                        boundary,
                        current.Key,
                        fileParameter.FileName ?? current.Key,
                        fileParameter.ContentType ?? "application/octet-stream"
                    });
                    stream.Write(HttpHelper4MultipartForm.encoding.GetBytes(s), 0, HttpHelper4MultipartForm.encoding.GetByteCount(s));
                    stream.Write(fileParameter.File, 0, fileParameter.File.Length);
                }
                else
                {
                    string s2 = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}", boundary, current.Key, current.Value);
                    stream.Write(HttpHelper4MultipartForm.encoding.GetBytes(s2), 0, HttpHelper4MultipartForm.encoding.GetByteCount(s2));
                }
            }
            string s3 = "\r\n--" + boundary + "--\r\n";
            stream.Write(HttpHelper4MultipartForm.encoding.GetBytes(s3), 0, HttpHelper4MultipartForm.encoding.GetByteCount(s3));
            stream.Position = 0L;
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            stream.Close();
            return array;
        }
    }
}
