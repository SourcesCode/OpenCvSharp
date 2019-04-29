using OpenCvSharp.Core;
using OpenCvSharp.ExtendApi.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace OpenCvSharp.ExtendApi
{
    public class FacePlusPlusApiHelper
    {
        static dynamic _setting;
        static FacePlusPlusApiHelper()
        {
            _setting = new
            {
                AccessKeyId = "UKClbXP9uWUP-qWYz77oc_6rFUogj2sc",
                AccessKeySecret = "ydpVgEBTBTSAmJCBukYiPjkr1cOpNiEp"
            };
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

        public static CommonExtendApiResult Detect(DetectRequest request)
        {
            var apiPath = "https://api-cn.faceplusplus.com/facepp/v3/detect";
            var result = new CommonExtendApiResult();

            Dictionary<string, object> verifyPostParameters = new Dictionary<string, object>();
            verifyPostParameters.Add("api_key", _setting.AccessKeyId);
            verifyPostParameters.Add("api_secret", _setting.AccessKeySecret);
            verifyPostParameters.Add("return_landmark", "1");//是否检测并返回人脸关键点。
            verifyPostParameters.Add("return_attributes", "gender,age,smiling,headpose,facequality,blur,eyestatus,emotion,ethnicity,beauty,mouthstatus,eyegaze,skinstatus");

            byte[] fileImage;
            using (Stream stream = File.Open(request.ImageFilePath, FileMode.Open))// 图片地址
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



}
